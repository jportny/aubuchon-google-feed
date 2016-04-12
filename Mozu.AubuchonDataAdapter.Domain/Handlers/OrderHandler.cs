using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
using Mozu.Api.Contracts.CommerceRuntime.Payments;
using Mozu.Api.Contracts.Customer.Credit;
using Mozu.Api.Resources.Commerce;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.Resources.Commerce.Customer.Credits;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Loyalty;
using Mozu.Api.Resources.Commerce.Orders.Attributedefinition;
using Mozu.Api.Resources.Commerce.Orders;
using Mozu.AubuchonDataAdapter.Domain.OrderHeaderService;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public interface IOrderHandler
    {
        IList<Order> Split(Order order);

        Task UpdateStoreCredits(IApiContext apiContext, string orderId);
        Task ExpireStoreCredit(IApiContext apiContext, string code);
        Task AddOrderAttribute(IApiContext apiContext, Order order, string attributeName, List<object> values);

        Task<IList<dynamic>> GetOrderStatus(IApiContext apiContext, string orderId);
    }

    public class OrderHandler : HandlerBase, IOrderHandler
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(OrderHandler));

        public OrderHandler(IAppSetting appSetting)
            : base(appSetting)
        {

        }


        public IList<Order> Split(Order order)
        {
            var shipToStoreAttrValues = order.GetAttribute(AttributeConstants.ShipToStore);
            var shipToStoreProducts = shipToStoreAttrValues != null ? Convert.ToString(shipToStoreAttrValues.Values.FirstOrDefault()) : null;





            if (!String.IsNullOrEmpty(shipToStoreProducts))
            {
                foreach (var productCode in shipToStoreProducts.Split(','))
                {
                    var pcode = productCode.Trim();
                    var orderItem = order.Items.FirstOrDefault(o => o.Product.ProductCode == pcode);
                    if (orderItem != null)
                    {
                        orderItem.FulfillmentMethod = "ShipToStore";
                    }
                }
            }

            var fulfillmentTypeGroup = (from orderItem in order.Items
                                        group orderItem by orderItem.FulfillmentMethod
                                            into g
                                            select new { FulfillmentType = g.Key, Orders = g.ToList() }).ToList();

            var orderSplits = new List<Order>();

            foreach (var ftype in fulfillmentTypeGroup)
            {
                var splitOrder = new Order
                {
                    CustomerAccountId = order.CustomerAccountId,
                    Items = new List<OrderItem>(),
                    DiscountTotal = 0,
                    DiscountedTotal = 0,
                    Subtotal = 0,
                    ShippingTotal = 0,
                    ShippingSubTotal = 0,
                    HandlingTotal = order.HandlingTotal,
                    DutyAmount = order.DutyAmount,
                    DutyTotal = order.DutyTotal,
                    OrderDiscounts = order.OrderDiscounts,
                    TaxTotal = 0,
                    Total = 0,
                    Payments = new List<Payment>(),
                    IsTaxExempt = order.IsTaxExempt,
                    CustomerTaxId = order.CustomerTaxId,
                    Attributes = order.Attributes,
                    ExtendedProperties = order.ExtendedProperties
                };



                foreach (var item in ftype.Orders)
                {
                    //Order level Calculations
                    splitOrder.Items.Add(item);
                    splitOrder.DiscountTotal += (item.Subtotal - item.TaxableTotal);
                    splitOrder.DiscountedTotal += item.TaxableTotal;
                    splitOrder.Subtotal += item.Subtotal;
                    splitOrder.TaxTotal += item.ItemTaxTotal;
                    splitOrder.ShippingSubTotal += item.ShippingTaxTotal;
                    splitOrder.Total += item.Total;
                    //Hack for additional handling in dutyamount field.
                    if (ftype.FulfillmentType != "Ship") continue;
                    var hazardous = item.Product.Properties.FirstOrDefault(
                        p =>
                            String.Equals(p.AttributeFQN, AttributeConstants.Hazardous,
                                StringComparison.CurrentCultureIgnoreCase));

                    if (hazardous != null && hazardous.Values != null)
                    {
                        var vl = hazardous.Values.FirstOrDefault();
                        if (vl == null || !(bool)vl.Value) continue;
                        if (order.DutyAmount != null && order.DutyAmount > 0)
                        {
                            splitOrder.Total += order.DutyAmount;
                        }
                        order.DutyAmount = 0;
                    }
                    //End of Calculations
                }

                //Additional Order Level Calculations/Assignments
                splitOrder.ShippingSubTotal = 0;

                if (ftype.FulfillmentType == "Ship")
                {
                    decimal? impact = 0;
                    if (order.ShippingDiscounts.Count > 0)
                    {
                        impact = order.ShippingDiscounts.First().Discount.Impact;
                    }
                    splitOrder.DiscountTotal += impact;

                    splitOrder.DiscountedTotal -= impact;
                    splitOrder.ShippingTotal += order.ShippingTotal;
                    splitOrder.ShippingSubTotal += order.ShippingSubTotal;
                }

                splitOrder.OrderNumber = order.OrderNumber;
                splitOrder.AuditInfo = order.AuditInfo;
                splitOrder.Notes = order.Notes;
                splitOrder.BillingInfo = order.BillingInfo;
                splitOrder.FulfillmentInfo = order.FulfillmentInfo;
                splitOrder.Id = String.Format("{0}_{1}", order.OrderNumber, ftype.FulfillmentType); //Assign Split Order Id



                orderSplits.Add(splitOrder);
            }

            if (!orderSplits.Any()) return orderSplits;
            var storeCredits =
                new Stack<Payment>(
                    order.Payments.Where(p => p.PaymentType == "StoreCredit")
                        .OrderByDescending(o => o.AmountRequested)
                        .ToList());
            var credits =
                order.Payments.Where(p => p.PaymentType == "CreditCard" && (p.Status == "Authorized" || p.Status == "Collected")).OrderBy(o => o.AmountRequested).ToList();
            var paypal =
                order.Payments.Where(p => p.PaymentType.Equals("Paypal", StringComparison.InvariantCultureIgnoreCase) || p.PaymentType.Equals("PaypalExpress", StringComparison.InvariantCultureIgnoreCase) || p.PaymentType.ToLower().Contains("paypal") && (p.Status == "Collected" || p.Status == "Authorized"))
                    .OrderByDescending(o => o.AmountRequested)
                    .ToList();

            decimal? appliedPaymentAmount = 0;

            //Apply Store Credits
            foreach (var ordr in orderSplits.OrderBy(o => o.Total))
            {

                var toCollect = ordr.Total;
                while (toCollect > 0 && storeCredits.Any())
                {
                    var sc = storeCredits.Pop();

                    if (toCollect < sc.AmountRequested)
                    {
                        var remainder = sc.Clone();
                        remainder.AmountRequested = (decimal)(sc.AmountRequested - toCollect);

                        if (toCollect != null)
                        {
                            sc.AmountRequested = (decimal)toCollect;
                            sc.AmountCollected = sc.AmountRequested;
                            appliedPaymentAmount += ordr.Total;
                            ordr.Payments.Add(sc);

                            storeCredits.Push(remainder);
                        }
                        toCollect -= sc.AmountRequested;
                        continue;
                    }

                    if (!(toCollect >= sc.AmountRequested)) continue;
                    sc.AmountCollected = sc.AmountRequested;
                    toCollect -= sc.AmountRequested;
                    ordr.Payments.Add(sc);

                }

                // amountCollected =  ordr.Payments.Sum(p => p.AmountRequested);

                //Add CC
                if (toCollect > 0 && credits.Any())
                {
                    var credit = credits.OrderBy(c => c.AmountRequested).FirstOrDefault();
                    var creditToAdd = credit.Clone();
                    if (credit != null) creditToAdd.Interactions = credit.Interactions;
                    if (toCollect < creditToAdd.AmountRequested)
                    {
                        if (toCollect != null)
                        {
                            creditToAdd.AmountRequested = (decimal)toCollect;
                            appliedPaymentAmount += ordr.Total;
                            ordr.Payments.Add(creditToAdd);

                        }
                        toCollect -= creditToAdd.AmountRequested;
                        //continue;
                    }

                    if (!(toCollect >= creditToAdd.AmountRequested)) continue;

                    ordr.Payments.Add(creditToAdd);
                }

                //Add paypal
                if (toCollect > 0 && paypal.Any())
                {
                    var pp = paypal.OrderBy(c => c.AmountRequested).FirstOrDefault();
                    var ppToAdd = pp.Clone();
                    if (pp != null) ppToAdd.Interactions = pp.Interactions;
                    if (toCollect < ppToAdd.AmountRequested)
                    {
                        if (toCollect != null)
                        {
                            ppToAdd.AmountRequested = (decimal)toCollect;
                            appliedPaymentAmount += ordr.Total;
                            ordr.Payments.Add(ppToAdd);

                        }
                        toCollect -= ppToAdd.AmountRequested;
                        continue;
                    }

                    if (!(toCollect >= ppToAdd.AmountRequested)) continue;

                    ordr.Payments.Add(ppToAdd);
                }
            }


            return orderSplits;
        }

        public async Task UpdateStoreCredits(IApiContext apiContext, string orderId)
        {
            var order = await (new OrderResource(apiContext)).GetOrderAsync(orderId);

            // get all store credit payments
            var payments = order.Payments.Where(p => p.BillingInfo.PaymentType == "StoreCredit").ToList();

            if (payments.Any())
            {
                // CustomerAccountId can be nothing
                if (order.CustomerAccountId.HasValue)
                {
                    var accountHandler = new AubuchonAccountHandler(AppSetting);

                    // get list of rewards to be reserved
                    var listOfRewards = payments.Where(p => p.Status != "Voided");

                    var rewardsList = listOfRewards.Select(reward => new RewardsToBeReserved { RewardsNo = reward.BillingInfo.StoreCreditCode }).ToList();



                    var authIDs = await accountHandler.RemoveCustomerRewardPoints(apiContext, (int)order.CustomerAccountId, rewardsList.ToArray());

                    var rewards = new List<object>();

                    foreach (var auth in authIDs)
                    {
                        rewards.Add(new { RNo = auth.Key, Auth = auth.Value });
                    }

                    //TODO: Move attribute name to a Constant
                    await AddOrderAttribute(apiContext, order, "reward-auth-ids", new List<object> { JsonConvert.SerializeObject(rewards) });
                }

                // expire store credit               
                foreach (var payment in payments)
                    await ExpireStoreCredit(apiContext, payment.BillingInfo.StoreCreditCode);
            }
        }

        public async Task ExpireStoreCredit(IApiContext apiContext, string code)
        {
            var creditResource = new CreditResource(apiContext);
            var creditTransactionResource = new CreditTransactionResource(apiContext);
            var credit = await creditResource.GetCreditAsync(code);

            if (credit == null || credit.CurrentBalance == 0) return;

            var creditTransaction = new CreditTransaction
            {
                ImpactAmount = -1 * credit.CurrentBalance,
                TransactionType = "Debit"
            };
            await creditTransactionResource.AddTransactionAsync(creditTransaction, code);
        }

        public async Task AddOrderAttribute(IApiContext apiContext, Order order, string attributeName, List<object> values)
        {
            var resource = new OrderAttributeResource(apiContext);
            var attributeResource = new AttributeResource(apiContext);
            var attribute = await attributeResource.GetAttributeAsync(attributeName);

            if (attribute == null) throw new Exception("Missing attribute 'Reward Auth IDs'");

            var attributeList = new List<OrderAttribute>();
            var orderAttribute = new OrderAttribute
            {
                Values = values,
                AttributeDefinitionId = attribute.Id,
                FullyQualifiedName = attribute.AttributeFQN
            };
            attributeList.Add(orderAttribute);

            if (order.Attributes.All(a => a.AttributeDefinitionId != attribute.Id))
                await resource.CreateOrderAttributesAsync(attributeList, order.Id);
            else
                await resource.UpdateOrderAttributesAsync(attributeList, order.Id, true);
        }

        public async Task<IList<dynamic>> GetOrderStatus(IApiContext apiContext, string orderId)
        {

            var client = new OrderHeaderServiceClient();

            if (client.ClientCredentials != null)
            {
                client.ClientCredentials.UserName.UserName = ServiceUsername;
                client.ClientCredentials.UserName.Password = ServicePassword;
            }

            var resource = new OrderResource(apiContext);
            var order = await resource.GetOrderAsync(orderId, responseFields: "OrderNumber,Attributes");
            var splitOrderNumberAttrib = order.Attributes.FirstOrDefault(a => String.Equals(a.FullyQualifiedName, AttributeConstants.SplitOrderNumbers, StringComparison.CurrentCultureIgnoreCase));
            if (splitOrderNumberAttrib == null || !splitOrderNumberAttrib.Values.Any()) return null;
            var splitOrderNumbers = Convert.ToString(splitOrderNumberAttrib.Values.First());
            var statuses = new List<dynamic>();
            foreach (var item in splitOrderNumbers.Split(','))
            {
                try
                {
                    var request = new getOrderHeaderByRefRequest(4, item.Trim());
                    var response = await client.getOrderHeaderByRefAsync(4, item.Trim());
                    if (response == null || response.orderDetail == null) continue;
                    var orderLines = response.orderDetail.orderHeader.orderShippingInfo.SelectMany(si => si.orderLines);

                    foreach (var lineDetail in orderLines.Select(ol => ol.orderLineDetails).SelectMany(orderLineDetails => orderLineDetails))
                    {
                        statuses.Add(new { lineid = lineDetail.orderLineDetailID, status = TranslateStatus(lineDetail.orderStatus.orderStatusCode) });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
            return statuses;
        }

        string TranslateStatus(string originalStatus)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            switch (originalStatus.ToLower())
            {
                case "approved":
                case "reserved":
                    {
                        return "Processing";
                    }
                case "partial":
                    {
                        return "Partially Backordered";
                    }
                case "error":
                case "under review":
                    {
                        return "Pending";
                    }
                case "complete":
                    {
                        return "Fulfilled";
                    }
                case "pending fulfillment":
                case "picked":
                case "picking":
                    {
                        return "Awaiting Fulfillment";
                    }
                case "shipping":
                    {
                        return "Pending Shipment";
                    }
                default:
                    {
                        return textInfo.ToTitleCase(originalStatus);
                    }

            }
        }
    }



}


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.CommerceRuntime.Discounts;
using Mozu.Api.Contracts.CommerceRuntime.Fulfillment;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
using Mozu.Api.Contracts.CommerceRuntime.Payments;
using Mozu.Api.Contracts.CommerceRuntime.Products;
using Mozu.Api.Contracts.Core;
using Mozu.Api.Contracts.Customer;
using Mozu.Api.Resources.Commerce.Admin;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace Mozu.AubuchonDataAdapter.Domain.Mappers
{
    public class OrderMapper
    {
        private static string _provideCode;
        private static string _xrefMerchantId;
        private readonly LocationResource _locationResource;

        public OrderMapper(IAppSetting appSetting, IApiContext apiContext)
        {
            _provideCode = (string)appSetting.Settings["ProviderCode"];
            _xrefMerchantId = (string)appSetting.Settings["XrefMerchantId"];
            _locationResource = new LocationResource(apiContext);
        }

        public async Task<OrderMessage> ToEdgeOrderExportMessage(Order order, CustomerAccount account, string splitOrderNumber, string mozuOrderId = null, StatusCode statusCode = StatusCode.New)
        {

            IList<CustomerSegment> customerSegments = account.Segments;
            var aubuchonCustomerId = account.ExternalId;

            var message = new OrderMessage();



            var head = new MessageHead
            {
                Messageid = order.OrderNumber.ToString(),
                Date = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"),
                AutoShipOrderHeadId = String.Empty,
                DestinationProviderCode = "MozuOrderimport",
                InterceptorName = String.Empty,
                ResponseDestinationProviderCode = "Mozu",
                Messagetype = "orderimport"
            };

            var sender = new MessageHeadSender
            {
                Systemid = _provideCode,
                Companyid = String.Empty,
                Replytoq = String.Empty
            };
            head.Sender = new MessageHeadSender[1];
            head.Sender[0] = new MessageHeadSender();
            head.Sender[0] = sender;

            var recipient = new MessageHeadRecipient
            {
                Companyid = _xrefMerchantId,
                Replytoq = String.Empty,
                Systemid = String.Empty
            };

            head.Recipient = new MessageHeadRecipient[1];
            head.Recipient[0] = recipient;


            message.Items = new object[2];
            message.Items[0] = head;

            var body = new OrderMessageBody();


            var orderHeader = new MessageBodyOrderImportOrderHeader
            {
                MerchantCode = "Aubuchon",
                ProjectCode = "Aubuchon",
                OrdertypeCode = "Sales",
                OrderedByUserName = String.Empty,
                CustodianUserName = String.Empty,
                Clientreference = splitOrderNumber,
                Referencefield1 = order.OrderNumber.ToString(), //TODO
                Referencefield2 = mozuOrderId,
                Referencefield3 = String.Empty,
                Referencefield4 = String.Empty,
                Referencefield5 = String.Empty,
                DistributionJobNumber = String.Empty,
                Comments = String.Empty,
                PriorityCode = String.Empty,
                WorkOrderTypeCode = String.Empty,
                AffiliateCode = String.Empty,
                MarketingSourceCode = String.Empty,
                ReleaseDate = order.AuditInfo.CreateDate.ToString(),
                OrderSourceCode = "Mozu",
                WorkOrderReference = String.Empty,
                CompleteByDate = String.Empty,
                PromotionCode = String.Empty,
                SalesTerritoryCode = String.Empty,
                BillingCode = "Consumer",
                BudgetCenterCode = String.Empty,
                ExceptionType = String.Empty,
                ExceptionMessage = String.Empty
            };


            

            var orderNote = order.Notes.FirstOrDefault();
            if (orderNote != null)
                orderHeader.Ordernotes = order.Notes.Any() ? orderNote.Text : String.Empty;
            orderHeader.Shipcomplete = String.Empty;
            orderHeader.OrderStatusCode = Enum.GetName(typeof(StatusCode), statusCode);

            var shippingMethodName = order.FulfillmentInfo.ShippingMethodName;

            var shipToStoreProp = order.ExtendedProperties != null ?
                                  order.ExtendedProperties.FirstOrDefault(
                                  p => p.Key.Equals("ship-to-store", StringComparison.InvariantCultureIgnoreCase)) : null;
            var isShipToStore = shipToStoreProp != null && shipToStoreProp.Value != null;



            //var orderItem = order.Items.FirstOrDefault();

            //if (orderItem != null)
            //{

            //    var isShipToStore = orderItem.Product.Options.Any(p => p.AttributeFQN.Equals(AttributeConstants.IsShipToStoreItem, StringComparison.InvariantCultureIgnoreCase) && p.StringValue.Equals("Yes", StringComparison.InvariantCultureIgnoreCase));
            //    if (isShipToStore || orderItem.FulfillmentMethod.Equals("Pickup",StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        orderHeader.ProviderCode = orderItem.FulfillmentLocationCode;
            //    }
            //}

            //orderHeader.OrderDetail = new MessageBodyOrderImportOrderHeaderOrderDetailOrderLine[order.Items.Count];

            if (order.BillingInfo == null || order.BillingInfo.BillingContact == null || order.BillingInfo.BillingContact.Address == null || order.BillingInfo.BillingContact.Address.Address1 == null)
            {
                if (account.Contacts != null)
                {
                    var customerBillingAddr =
                        account.Contacts.FirstOrDefault(c => c.Types.Any(t => t.Name == "Billing" && t.IsPrimary)) ??
                        account.Contacts.FirstOrDefault(c => c.Types.Any(t => t.Name == "Billing")) ??
                        account.Contacts.FirstOrDefault(c => c.Types.Any(t => t.Name == "Shipping")) ??
                        account.Contacts.FirstOrDefault();

                    if (customerBillingAddr != null)
                    {
                        order.BillingInfo = new BillingInfo
                        {
                            BillingContact = new Contact
                            {
                                FirstName = customerBillingAddr.FirstName,
                                LastNameOrSurname = customerBillingAddr.LastNameOrSurname,
                                Address =
                                    new Address
                                    {
                                        Address1 = customerBillingAddr.Address.Address1,
                                        Address2 = customerBillingAddr.Address.Address2,
                                        Address3 = customerBillingAddr.Address.Address3,
                                        Address4 = customerBillingAddr.Address.Address4,
                                        CityOrTown = customerBillingAddr.Address.CityOrTown,
                                        StateOrProvince = customerBillingAddr.Address.StateOrProvince,
                                        PostalOrZipCode = customerBillingAddr.Address.PostalOrZipCode,
                                        CountryCode = customerBillingAddr.Address.CountryCode
                                    },
                                PhoneNumbers =
                                    new Phone
                                    {
                                        Home = customerBillingAddr.PhoneNumbers.Home,
                                        Work = customerBillingAddr.PhoneNumbers.Work,
                                        Mobile = customerBillingAddr.PhoneNumbers.Mobile
                                    }
                            }
                        };
                    }
                }
            }

            if (order.BillingInfo != null)
            {
                var billingContact = order.BillingInfo.BillingContact;

                if (order.BillingInfo.BillingContact != null && order.BillingInfo.BillingContact.Address == null)
                {
                    var orderItem = order.Items.FirstOrDefault(i => !String.IsNullOrEmpty(i.FulfillmentLocationCode));
                    var fulfillmentLocationCode = orderItem != null ? orderItem.FulfillmentLocationCode : "991";
                    var fulfillmentInfo = await GetLocation(fulfillmentLocationCode);
                    billingContact = fulfillmentInfo.FulfillmentContact;
                }
                if (billingContact != null)
                {
                    var billToAddress = new MessageBodyOrderImportOrderHeaderBillToAddress
                    {
                        Salutation = String.Empty,
                        Organization = new MessageBodyOrderImportOrderHeaderBillToAddressOrganization[] { },
                        Namefirst = billingContact.FirstName,
                        Namelast = billingContact.LastNameOrSurname,
                        NameMiddle = String.Empty,
                        Address1 = billingContact.Address.Address1,
                        Address2 = billingContact.Address.Address2,
                        Address3 = billingContact.Address.Address3,
                        City = billingContact.Address.CityOrTown,
                        StateProvince = billingContact.Address.StateOrProvince,
                        PostalCode = billingContact.Address.PostalOrZipCode,
                        CountryCode = billingContact.Address.CountryCode,
                        PhoneWork = billingContact.PhoneNumbers.Work,
                        PhoneHome = billingContact.PhoneNumbers.Home,
                        PhoneCell = billingContact.PhoneNumbers.Mobile,
                        PhonePager = String.Empty,
                        PhoneOther = String.Empty,
                        Fax = String.Empty,
                        Email = billingContact.Email,
                        Username = String.Empty,
                        TypeCode = "Customer",
                        MemberId = String.Empty,
                        MemberNumber = aubuchonCustomerId,  //order.ExternalId <- This always appears to be null although documentation suggests otherwise
                        ExternMemberXRef = order.CustomerAccountId.ToString(),
                        StorePaymentInfo = String.Empty,
                        TaxNumber = order.IsTaxExempt != null && order.IsTaxExempt.Value ? order.CustomerTaxId : String.Empty,
                        OriginCode = String.Empty,
                        SourceCode = String.Empty,
                        PromotionCode = String.Empty,
                        PriceListCode = String.Empty,
                        IsResidential = "1"


                    };

                    if (String.IsNullOrEmpty(billToAddress.PhoneWork))
                    {
                        billToAddress.PhoneWork = !String.IsNullOrEmpty(billToAddress.PhoneCell)
                            ? billToAddress.PhoneCell
                            : billToAddress.PhoneHome;

                    }

                    billToAddress.IsResidential = billingContact.Address.AddressType == null ||
                                                  billingContact.Address.AddressType == "Commercial"
                        ? "0"
                        : "1";



                    orderHeader.BillToAddress = new MessageBodyOrderImportOrderHeaderBillToAddress[1];
                    orderHeader.BillToAddress[0] = billToAddress;
                }
            }

            orderHeader.Payments = new MessageBodyOrderImportOrderHeaderPayments[1];
            orderHeader.Payments[0] = new MessageBodyOrderImportOrderHeaderPayments
            {
                MessageBodyOrderImportOrderHeaderPaymentInfo =
                    new MessageBodyOrderImportOrderHeaderPaymentInfo[order.Payments.Count]
            };


            //Get Rewards Auth Id if any




            var piIndex = 0;
            foreach (var payment in order.Payments)
            {

                var paymentInfo = new MessageBodyOrderImportOrderHeaderPaymentInfo
                {
                    //CreditCardNumber = String.Empty,
                    //Cvv = String.Empty,
                    ExpiryMonth = payment.BillingInfo != null && payment.BillingInfo.Card != null && payment.BillingInfo.Card.ExpireMonth != 0 ? Convert.ToString(payment.BillingInfo.Card.ExpireMonth) : String.Empty,
                    ExpiryYear = payment.BillingInfo != null && payment.BillingInfo.Card != null && payment.BillingInfo.Card.ExpireYear != 0 ? Convert.ToString(payment.BillingInfo.Card.ExpireMonth) : String.Empty,
                    MemberUsername = String.Empty,
                    Taxnumber = String.Empty,
                    Authorizercode = "PayPalEC",
                    Primaryfororder = "0",
                    Responsesubcode = String.Empty,
                    Cardcoderesponsecode = String.Empty,
                    Notes = String.Empty,
                    Transactionfee = String.Empty,
                    Exchangerate = String.Empty,
                    Payerstatus = String.Empty,
                    //Waivecharges = String.Empty,
                    Paymenttoken = String.Empty,
                    Gatewaytransactionindex = String.Empty,
                    Verifysign = String.Empty,
                    Pplsubscriberid = String.Empty,
                    Cardcode = String.Empty,
                    Pmtterm = String.Empty,
                    Pmtmethod = String.Empty,
                    Thirdpartybillno = String.Empty,
                    CreditCardName = String.Empty,
                    CreditCardSuffix = String.Empty,
                    RoutingNumber = String.Empty,
                    Settlecurrencycode = "USD",
                    Paymentdate = String.Empty,
                    Purchaseorderdate = String.Empty,
                    Purchaseorderexp = String.Empty,
                    Pendingreason = String.Empty,
                    Paymentstatuscode = "Completed",
                    Thirdpartyaccountnumber = String.Empty,
                    Currencycode = "USD",

                };

                var interaction = payment.Interactions != null ?
                    (payment.Interactions
                        .FirstOrDefault(i => !String.IsNullOrEmpty(i.GatewayTransactionId))
                            ?? payment.Interactions.FirstOrDefault()) : null;

                if (interaction != null)
                {
                    paymentInfo.Currencycode = interaction.CurrencyCode;
                    paymentInfo.Addressvalidationcode = interaction.GatewayAVSCodes;
                    paymentInfo.Paymentdate = interaction.InteractionDate.ToString();
                }

                if (payment.PaymentType == "CreditCard" || payment.PaymentType == "PaypalExpress" || payment.PaymentType == "PayPal" || payment.PaymentType.ToLower().Contains("paypal"))
                {
                    paymentInfo.Paymenttypename = "CreditCard";
                    if (interaction != null)
                        paymentInfo.Authtrnno = interaction.GatewayTransactionId;
                }
                //else if (payment.PaymentType == "PaypalExpress" || order.BillingInfo.PaymentType == "PayPal")
                //{
                //    paymentInfo.Paymenttypename = "Paypal";
                //    if (interaction != null)
                //        paymentInfo.Authtrnno = interaction.GatewayTransactionId;
                //}
                else if (payment.PaymentType == "StoreCredit")
                {
                    paymentInfo.Paymenttypename = "LoyaltyRewards";
                    paymentInfo.Thirdpartyaccountnumber = payment.BillingInfo.StoreCreditCode;


                }


                if (payment.PaymentType == "CreditCard")
                {
                    //Do not send CC number
                    paymentInfo.CreditCardNumber = String.Empty;
                    paymentInfo.CreditCardSuffix= payment.BillingInfo.Card.CardNumberPartOrMask.Replace("*", "");
                    paymentInfo.ExpiryMonth = payment.BillingInfo.Card.ExpireMonth.ToString(CultureInfo.InvariantCulture);
                    paymentInfo.ExpiryYear = payment.BillingInfo.Card.ExpireYear.ToString(CultureInfo.InvariantCulture);
                    paymentInfo.Authorizercode = "Element";
                    paymentInfo.Transactiontypecode = "AUTH_ONLY";
                    paymentInfo.Amount = payment.AmountRequested.ToString(CultureInfo.InvariantCulture); //TODO: Check if it is authorized amount
                    paymentInfo.Settleamount = payment.AmountRequested.ToString(CultureInfo.InvariantCulture);

                    paymentInfo.CreditCardType = GetCreditCardType(payment.BillingInfo.Card.PaymentOrCardType);

                    if (interaction != null)
                    {
                        paymentInfo.Responsecode = interaction.GatewayResponseCode;
                        paymentInfo.Responsereasoncode = interaction.GatewayResponseCode;
                        paymentInfo.Responsereasontext = interaction.GatewayResponseText;
                        paymentInfo.Addressvalidationcode = interaction.GatewayAVSCodes;
                    }
                    paymentInfo.Approvalcode = String.Empty;
                }

                if (payment.PaymentType == "PayPal" || payment.PaymentType == "PaypalExpress" || payment.PaymentType.ToLower().Contains("paypal"))
                {
                    paymentInfo.Transactiontypecode = "AUTH_CAPTURE";
                    paymentInfo.Amount = payment.AmountRequested.ToString(CultureInfo.InvariantCulture);
                    paymentInfo.Settleamount = payment.AmountRequested.ToString(CultureInfo.InvariantCulture);
                    paymentInfo.Responsecode = "0";
                    paymentInfo.Responsereasoncode = String.Empty;
                    paymentInfo.Responsereasontext = String.Empty;
                    paymentInfo.Pendingreason = "authorization";
                    if (interaction != null)
                    {
                        paymentInfo.Approvalcode = interaction.GatewayTransactionId;
                    }
                }

                if (payment.PaymentType == "StoreCredit")
                {
                    paymentInfo.Transactiontypecode = "AUTH_ONLY";
                    paymentInfo.Authorizercode = "LoyaltyRewards";
                    paymentInfo.Currencycode = String.Empty;
                    paymentInfo.Amount = payment.AmountCollected.ToString(CultureInfo.InvariantCulture); //TODO: Check if correct
                    paymentInfo.Settleamount = payment.AmountCollected.ToString(CultureInfo.InvariantCulture);
                    paymentInfo.Responsecode = "1";


                    var rewardAuthAttr = order.GetAttribute(AttributeConstants.RewardAuthIds);

                    paymentInfo.Authtrnno = GetAuthId(rewardAuthAttr, payment.BillingInfo.StoreCreditCode);

                }

                if ((order.Payments.Count > 1 && (payment.PaymentType.Equals("Credit", StringComparison.InvariantCultureIgnoreCase) || payment.PaymentType.Equals("CreditCard", StringComparison.InvariantCultureIgnoreCase))) || order.Payments.Count == 1)
                {
                    paymentInfo.Primaryfororder = "1";
                }
                if (payment.BillingInfo.Card != null)
                {
                    paymentInfo.CreditCardType = GetCreditCardType(payment.BillingInfo.Card.PaymentOrCardType);

                }


                //paymentInfo.Primaryfororder
                //TODO: Payment info for Rewards
                orderHeader.Payments[0].MessageBodyOrderImportOrderHeaderPaymentInfo[piIndex] = paymentInfo;
                piIndex++;
            }
            
            

            //Alternate person pickup
            var alternatePersonAttr =
                order.Attributes.FirstOrDefault(
                    a =>
                        a.FullyQualifiedName.Equals(AttributeConstants.AlternatePersonPickup,
                            StringComparison.InvariantCultureIgnoreCase));


            orderHeader.DynamicField = new DynamicField[3];
            orderHeader.DynamicField[0] = new DynamicField
            {
                DynamicFieldCode = "aubuchon_customer_id_order",
                DynamicFieldLabel = "Aubuchon Customer ID",
                DynamicFieldValue = account.ExternalId ?? ""
            };

            var alternatePersonAttrValue = alternatePersonAttr != null && alternatePersonAttr.Values != null
                ? Convert.ToString(alternatePersonAttr.Values.FirstOrDefault())
                : String.Empty;

            var alternatePersonValueArr = alternatePersonAttrValue.Split(',');

            if (alternatePersonValueArr.Count() == 4)
            {
                orderHeader.DynamicField[1] = new DynamicField
                {
                    DynamicFieldCode = "mobileNumber",
                    DynamicFieldLabel = "Mobile Number for SMS Notifications",
                    DynamicFieldValue = alternatePersonValueArr[1]
                };

                var smsOptIn = alternatePersonValueArr[3] != null && alternatePersonValueArr[3].EndsWith("True");

                orderHeader.DynamicField[2] = new DynamicField
                {
                    DynamicFieldCode = "notifyBySMS",
                    DynamicFieldLabel = "SMS Opt-In?",
                    DynamicFieldValue = smsOptIn ? "Yes" : "No"
                };
            }
            var itemCount = 0;
            orderHeader.OrderDetail = new MessageBodyOrderImportOrderHeaderOrderDetail[order.Items.Count];


            foreach (var item in order.Items)
            {


                //var hasShipToStoreAttrib =
                //    item.Product.Properties.Any(p => p.AttributeFQN.Equals(AttributeConstants.IsShipToStoreItem,StringComparison.InvariantCultureIgnoreCase) && p.Values.Any(v => v.Value.Equals(true)));

                orderHeader.OrderDetail[itemCount] = new MessageBodyOrderImportOrderHeaderOrderDetail
                {
                    OrderLine = new MessageBodyOrderImportOrderHeaderOrderDetailOrderLine()
                };
                var orderLine = orderHeader.OrderDetail[itemCount].OrderLine;
                //Shipping Info
                var shippingInfo = new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineShippingInfo
                {
                    ShippingHandling = Convert.ToString(order.ShippingTotal),
                    Discount = null,
                    DelayShipUntil = String.Empty,
                    ShipperAttentionNeeded = String.Empty,
                    DateNeeded = String.Empty,
                    LastDropDate = String.Empty,
                    DeclaredValue = String.Empty,
                    InsuranceCharge = String.Empty,
                    GiftMessage = String.Empty,
                    ShippingHandlingTax = item.ShippingTaxTotal.ToString(),
                    ShipMethodCode =
                        GetShipMethodCode(customerSegments, item.FulfillmentMethod, shippingMethodName, isShipToStore),
                    ShipNotes = alternatePersonAttr != null && alternatePersonAttr.Values != null
                        ? Convert.ToString(alternatePersonAttr.Values.FirstOrDefault())
                        : String.Empty
                };

                if (isShipToStore ||
                    item.FulfillmentMethod.Equals("Pickup", StringComparison.InvariantCultureIgnoreCase))
                {
                    orderHeader.ProviderCode = shipToStoreProp != null ? shipToStoreProp.Value : item.FulfillmentLocationCode;
                    shippingInfo.ProviderCode = shipToStoreProp != null ? shipToStoreProp.Value : item.FulfillmentLocationCode;
                }
                else
                {
                    shippingInfo.ShippingHandling = order.DutyAmount != null ? Convert.ToString(order.ShippingTotal + order.DutyAmount) : Convert.ToString(order.ShippingTotal);
                }

                switch (item.FulfillmentMethod)
                {
                    case "PickUp":
                        shippingInfo.ProviderCode = String.Empty;
                        break;
                    //needs to be empty for Ship To Home and Ship To Store orders so the EDGE Provider Optimization engine can choose the optimal provider
                    case "Ship":
                        shippingInfo.ProviderCode = String.Empty;
                        break;
                    case "ShipToStore":
                        shippingInfo.ProviderCode = String.Empty;
                        break;
                }
                orderLine.ShippingInfo =
                    new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineShippingInfo[1];
                orderLine.ShippingInfo[0] = shippingInfo;
                //orderHeader.OrderDetail[itemCount].ShippingInfo[0] = shippingInfo;

                //Catalog
                var catalog = new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineCatalog
                {
                    Catalogitem =  String.IsNullOrEmpty(item.Product.VariationProductCode) ? item.Product.ProductCode : item.Product.VariationProductCode,
                    Gift = String.Empty,
                    ReleaseDate = String.Empty,
                    UpdateTax = String.Empty,
                    UpdateShipping = String.Empty,
                    Sku = item.Product.ProductCode,
                    Quantity = item.Quantity,
                    UnitPrice = (item.TaxableTotal / item.Quantity).ToString(),
                    UnitTax = String.Empty,
                    LineTax = item.ItemTaxTotal.ToString()
                };

                orderLine.Catalog =
                    new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineCatalog[1];
                orderLine.Catalog[0] = catalog;
                //orderHeader.OrderDetail[itemCount].Catalog[0] = catalog;


                //Ship To Address
                var shipToAddress = new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineShipToAddress
                {
                    PhonePager = String.Empty,
                    PhoneOther = String.Empty,
                    Fax = String.Empty,

                    IsResidential = "0"
                };

                var locationInfo = new FulfillmentInfo();

                if (item.FulfillmentMethod == "Pickup")
                {
                    //Ship To Address
                    shipToAddress.IsResidential = "0";

                    //TODO: what should be the mapping if fulfillmentcontact is missing
                    locationInfo = await GetLocation(item.FulfillmentLocationCode);
                }
                else if (isShipToStore)
                {
                    shipToAddress.IsResidential = "0";

                    locationInfo = await GetLocation(shipToStoreProp.Value);
                }
                else
                {
                    //Ship To Address
                    shipToAddress.IsResidential = "1";
                    locationInfo = order.FulfillmentInfo;
                    if (item.HandlingAmount > 0)
                    {
                        orderLine.ShippingInfo = new[] { new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineShippingInfo { ShippingHandling = Convert.ToString(item.ShippingTotal + item.HandlingAmount) } };

                    }
                }



                if (locationInfo.FulfillmentContact != null)
                {
                    shipToAddress.Email = locationInfo.FulfillmentContact.Email;
                    shipToAddress.Namefirst = locationInfo.FulfillmentContact.FirstName;
                    shipToAddress.Namelast = locationInfo.FulfillmentContact.LastNameOrSurname;
                    shipToAddress.Address1 = locationInfo.FulfillmentContact.Address.Address1;
                    shipToAddress.Address2 = locationInfo.FulfillmentContact.Address.Address2;
                    shipToAddress.Address3 = locationInfo.FulfillmentContact.Address.Address3;
                    shipToAddress.City = locationInfo.FulfillmentContact.Address.CityOrTown;
                    shipToAddress.StateProvince = locationInfo.FulfillmentContact.Address.StateOrProvince;
                    shipToAddress.PostalCode = locationInfo.FulfillmentContact.Address.PostalOrZipCode;
                    shipToAddress.CountryCode = locationInfo.FulfillmentContact.Address.CountryCode;
                    if (locationInfo.FulfillmentContact.PhoneNumbers != null)
                    {
                        shipToAddress.PhoneWork = locationInfo.FulfillmentContact.PhoneNumbers.Work;
                        shipToAddress.PhoneHome = locationInfo.FulfillmentContact.PhoneNumbers.Home;
                        shipToAddress.PhoneCell = locationInfo.FulfillmentContact.PhoneNumbers.Mobile;
                    }
                }

                if (String.IsNullOrEmpty(shipToAddress.PhoneWork))
                {
                    shipToAddress.PhoneWork = !String.IsNullOrEmpty(shipToAddress.PhoneCell)
                        ? shipToAddress.PhoneCell
                        : shipToAddress.PhoneHome;

                }

                orderLine.ShipToAddress =
                    new MessageBodyOrderImportOrderHeaderOrderDetailOrderLineShipToAddress[1];
                orderLine.ShipToAddress[0] = shipToAddress;

                //var attribInstantRewards =
                //    item.Product.Properties.FirstOrDefault(p => p.AttributeFQN.Equals(AttributeConstants.InstantRewardsAdmin, StringComparison.InvariantCultureIgnoreCase));
                //var attribFreeAssembly =
                //    item.Product.Options.FirstOrDefault(p => p.AttributeFQN.Equals(AttributeConstants.FreeAssembly, StringComparison.InvariantCultureIgnoreCase));


                orderLine.DynamicField = GetDynamicFields(order, item, item.Product.Properties,
                    item.Product.Options);




                //orderHeader.OrderDetail[itemCount].DynamicField[0] = dynamicField;
                //orderHeader.OrderDetail[0] = orderLine;
                itemCount++;
            }



            body.OrderImport = new MessageBodyOrderImportOrderHeader[1];
            //body.OrderImport[0] = new MessageBodyOrderImportOrderHeader[1];
            body.OrderImport[0] = orderHeader;

            message.Items[1] = body;

            return message;
        }
        private static string GetAuthId(OrderAttribute attribAuthId, string rewardsNumber)
        {
            try
            {
                if (attribAuthId == null || !attribAuthId.Values.Any()) return String.Empty;
                var json = attribAuthId.Values.First().ToString();

                var rewardAuthType = new[] { new { RNo = "", Auth = "" } };

                var result = JsonConvert.DeserializeAnonymousType(json, rewardAuthType);

                var rewardData = result.FirstOrDefault(r => r.RNo == rewardsNumber);
                return rewardData != null ? rewardData.Auth : String.Empty;
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        private async Task<FulfillmentInfo> GetLocation(string locationCode)
        {
            var location = await _locationResource.GetLocationAsync(locationCode,"Name,Address, Description,ShippingOriginContact,Phone");
            
            var fulfillmentInfo = new FulfillmentInfo
            {
                FulfillmentContact = new Contact
                {
                    Address = location.Address, 
                    FirstName = location.ShippingOriginContact != null ? location.ShippingOriginContact.FirstName : location.Name,
                    LastNameOrSurname = location.ShippingOriginContact != null ? location.ShippingOriginContact.LastNameOrSurname : location.Name, 
                    CompanyOrOrganization = location.Description, 
                    PhoneNumbers = new Phone() { Work = location.Phone }, 
                    Email = location.ShippingOriginContact != null ? location.ShippingOriginContact.Email : "help@aubuchon.com"
                }
            };
            return fulfillmentInfo;
        }

        private static DynamicField[] GetDynamicFields(Order order, OrderItem item, IEnumerable<ProductProperty> productProperties, IEnumerable<ProductOption> productOptions)
        {
            var fields = new List<DynamicField>();

            foreach (var prop in productProperties.Where(prop => prop != null).Where(prop => prop.AttributeFQN.Equals(AttributeConstants.InstantRewardsAdmin, StringComparison.InvariantCultureIgnoreCase)))
            {
                var dynamicField = new DynamicField
                {
                    DynamicFieldCode = "instant_reward_admin",
                    DynamicFieldLabel = "Instant Rewards Eligible"
                };

                var productPropertyValue = prop.Values.FirstOrDefault();
                if (productPropertyValue != null)
                    dynamicField.DynamicFieldValue = productPropertyValue.StringValue;

                fields.Add(dynamicField);
            }

            fields.AddRange(
                productOptions.Where(option => option.AttributeFQN.Equals(AttributeConstants.FreeAssembly,
                    StringComparison.InvariantCultureIgnoreCase) && option.ShopperEnteredValue != null && option.ShopperEnteredValue.Equals(true)).Select(option => new DynamicField
            {
                DynamicFieldCode = "assembled_yes_no",
                DynamicFieldLabel = "Should we assemble this?",
                DynamicFieldValue = Convert.ToString(option.ShopperEnteredValue)
            }));


            var productCode = String.IsNullOrEmpty(item.Product.VariationProductCode) ? item.Product.ProductCode : item.Product.VariationProductCode;
            if (productCode.StartsWith("paint-"))
            {
                var code = productCode.Substring(productCode.IndexOf("paint-", StringComparison.Ordinal) + 1);
                var dynamicField = new DynamicField
                {
                    DynamicFieldCode = "paint_color_and_code",
                    DynamicFieldLabel = "Paint Color and Code",
                    DynamicFieldValue = code
                };

                fields.Add(dynamicField);
            }

            var orderNoPlusLine = String.Format("{0}|{1}|{2}", order.OrderNumber, item.LineId, item.Id);
            var dynamicFld = new DynamicField
            {
                DynamicFieldCode = "mozu_order_and_line_item_num",
                DynamicFieldLabel = "Mozu Order Number and Line Item Number",
                DynamicFieldValue = orderNoPlusLine
            };


            fields.Add(dynamicFld);

            
            var orderDiscountIds = order.OrderDiscounts != null ? order.OrderDiscounts.Select(d => d.Discount.Name).ToList() : new List<string>();
            var shippingDiscounts = order.ShippingDiscounts != null ? order.ShippingDiscounts.Select(d => d.Discount.Discount.Name).ToList() : new List<string>();
            var orderItemDiscounts = item.ProductDiscounts != null ? item.ProductDiscounts.Select(i => i.Discount.Name).ToList() : new List<string>();
            var shippingItemDiscounts = item.ShippingDiscounts != null ? item.ShippingDiscounts.Select(i => i.Discount.Discount.Name).ToList() : new List<string>();
            var discountNames = new List<string>();

            discountNames.AddRange(orderDiscountIds);
            discountNames.AddRange(orderItemDiscounts);
            discountNames.AddRange(shippingDiscounts);
            discountNames.AddRange(shippingItemDiscounts);

            if (discountNames.Any())
            {
                var discountFields = new DynamicField
                {
                    DynamicFieldCode = "mozu_discount_name",
                    DynamicFieldLabel = "Order and(or) Item level discounts",
                    DynamicFieldValue = String.Join(" | ", discountNames)
                };
                fields.Add(discountFields);
            }

            return fields.ToArray();

        }

        private static string GetCreditCardType(string paymentOrCardType)
        {
            if (String.IsNullOrWhiteSpace(paymentOrCardType))
                return String.Empty;
            switch (paymentOrCardType.ToLower())
            {
                case "visa":
                case "vi":
                    return "VI";
                case "discover":
                case "dc":
                    return "DC";
                case "mastercard":
                case "mc":
                    return "MC";
                case "amex":
                case "ax":
                    return "AX";
                case "paypal":
                case "paypalexpress":
                case "paypalexpress2":
                    return String.Empty;
            }
            return String.Empty;
        }


        private static string GetShipMethodCode(IEnumerable<CustomerSegment> customerSegments, string fulfillmentType, string shipMethod, bool isShipToStore)
        {
            if (fulfillmentType.Equals("Ship", StringComparison.InvariantCultureIgnoreCase))
            {
                if (isShipToStore)
                {
                    return "ship-to-store";
                }

                switch (shipMethod)
                {
                    case "UPS Ground":
                        return "ship-to-home-standard";
                    case "UPS 2nd Day Air":
                    case "UPS Second Day Air®":
                    case "UPS Second Day Air":
                        return "ship-to-home-expedited";
                    case "UPS Next Day Air":
                    case "UPS Next Day Air Saver®":
                        return "ship-to-home-express";
                    default:
                        return "ship-to-home-standard";
                }
            }

            //var isEmployee = customerSegments.Any(s => s.Name.Equals("Employee", StringComparison.InvariantCultureIgnoreCase));
            //return isEmployee ? "home-office-pickup" : "rapid-pickup";

            return "rapid-pickup";

        }

    }


}

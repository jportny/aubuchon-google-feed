using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Mozu.Api;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
using Mozu.Api.Resources.Commerce.Orders;
using Mozu.Api.Resources.Commerce.Orders.Attributedefinition;
using Mozu.AubuchonDataAdapter.Domain;
using Mozu.AubuchonDataAdapter.Domain.Handlers;

namespace Mozu.AubuchonDataAdapter.Web.Controllers
{
    [AubCors(headers: "origin, accept, x-vol-user-claims, x-vol-master-catalog, x-vol-app-claims, x-vol-currency, x-vol-site, content-type, x-vol-tenant, x-vol-locale, x-vol-catalog,x-vol-accountid", methods: "POST")]
    [ApiContextFilter]
    public class OrderController : ApiController
    {
        private readonly IOrderHandler _orderHandler;
        public OrderController(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }

        [HttpPost]
        [ValidateUserClaimFilter]
        public async Task<HttpResponseMessage> Status(int tenantId, int siteId, int accountId, string orderId)
        {
            var apiContext = new ApiContext(tenantId, siteId);
            try
            {
                var orderStatus = await _orderHandler.GetOrderStatus(apiContext, orderId);
                return Request.CreateResponse(HttpStatusCode.OK, orderStatus);
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        [HttpPost]
        //[ValidateUserClaimFilter]
        public async Task<HttpResponseMessage> Pickup(Models.AlternatePersonModel model)
        {
            try
            {
                object apiCtxtObj;

                Request.Properties.TryGetValue("ApiContext", out apiCtxtObj);

                var apiContext = (ApiContext)apiCtxtObj;

                var attributeResource = new AttributeResource(apiContext);
                var pickupAttr = await attributeResource.GetAttributeAsync(AttributeConstants.AlternatePersonPickup);

                var pickupValue = String.Format("{0} , {1}, {2} {3}, SMS {4}", model.Name, model.Phone, model.PickupDate,
                    model.Ampm, model.Notify == "1" ? "True" : "False");

                var formatted = String.Format("{0} {1} {2} {3}", model.Name, model.PickupDate, model.Ampm, model.Phone);

                var orderAttribute = new OrderAttribute
                {
                    AttributeDefinitionId = pickupAttr.Id,
                    FullyQualifiedName = AttributeConstants.AlternatePersonPickup,
                    Values = new List<object> { pickupValue }
                };

                var orderAttrResource = new OrderAttributeResource(apiContext);

                await orderAttrResource.CreateOrderAttributesAsync(new List<OrderAttribute> { orderAttribute }, model.OrderId);
                return Request.CreateResponse(HttpStatusCode.OK, formatted);

            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }



    }
}

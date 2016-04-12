using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Common.Logging;
using Mozu.Api;
using Mozu.Api.Contracts.CommerceRuntime.Orders;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Resources.Commerce;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.Resources.Commerce.Orders;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Mozu.AubuchonDataAdapter.Domain.Mappers;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class OrderEventProcessor : IEventProcessor
    {
        private readonly IOrderHandler _orderHandler;
        private readonly IAppSetting _appSetting;
        private readonly IEventHandler _eventHandler;
        private readonly ILog _logger = LogManager.GetLogger(typeof(OrderEventProcessor));
        public OrderEventProcessor(IOrderHandler orderHandler, IEventHandler eventHandler, IAppSetting appSetting)
        {
            _orderHandler = orderHandler;
            _eventHandler = eventHandler;
            _appSetting = appSetting;
        }
        public async Task ProcessEvent(IApiContext apiContext, Event evt, StatusCode statusCode = StatusCode.Active)
        {
            try
            {
                await _orderHandler.UpdateStoreCredits(apiContext, evt.EntityId);

                var order = await (new OrderResource(apiContext)).GetOrderAsync(evt.EntityId);
                var customer =
                    await
                        (new CustomerAccountResource(apiContext)).GetAccountAsync(
                            Convert.ToInt32(order.CustomerAccountId), "Id, Segments, Contacts, ExternalId, TaxExempt, TaxId");


                var mozuOrderId = order.Id;

                var splits =  _orderHandler.Split(order);
                var orderMapper = new OrderMapper(_appSetting,apiContext);

                var splitNos = new List<String>();

                foreach (var split in splits)
                {
                    var message = await orderMapper.ToEdgeOrderExportMessage(split, customer, split.Id, mozuOrderId);
                    var orderXml = message.ToXml();
                    var filePath = String.Format(@"C:\temp\{0}.xml", split.Id);
                    var xdoc = new XmlDocument();
                    xdoc.LoadXml(orderXml);
                    xdoc.Save(filePath);

                    var sftpHandler = new SftpHandler(_appSetting);
                    sftpHandler.Push(filePath, "/home/MozuAubuchonSFTP/inbox/JaggedPeak/Mozu/OrderImport/");

                    splitNos.Add(split.Id);
                }


                //Save split numbers to custom attribute
                if (splitNos.Any())
                {
                    var orderAttribResource = new OrderAttributeResource(apiContext);
                    await orderAttribResource.CreateOrderAttributesAsync(new List<OrderAttribute>()
                    {
                        new OrderAttribute
                        {
                            FullyQualifiedName = AttributeConstants.SplitOrderNumbers,
                            Values = new List<object> {String.Join(",", splitNos.ToArray())}
                        }
                    }, order.Id);
                }
                //Set Event as Processed
                var aubEvent = evt.ToAubEvent();
                aubEvent.Status = EventStatus.Processed.ToString();
                aubEvent.ProcessedDateTime = DateTime.Now;
                await _eventHandler.UpdateEvent(apiContext.TenantId, aubEvent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

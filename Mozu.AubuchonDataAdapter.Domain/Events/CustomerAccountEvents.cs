using System;
using System.Linq;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Events;
using Mozu.Api.Resources.Commerce.Customer;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Handlers;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class CustomerAccountEvents : ICustomerAccountEvents
    {

        private readonly IEventProcessor _eventProcessor;
        private readonly IEventHandler _eventHandler;
        private readonly bool _isLive;
        public CustomerAccountEvents(IEventProcessor eventProcessor, IEventHandler eventHandler, IAppSetting appSetting)
        {
            _eventProcessor = eventProcessor;
            _eventHandler = eventHandler;
            _isLive = appSetting.Settings.ContainsKey("IsLive") && Convert.ToBoolean(appSetting.Settings["IsLive"]);

        }
        public void Created(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task CreatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            if (!await ShouldIgnoreCustomer(apiContext, Convert.ToInt32(eventPayLoad.EntityId)))
            {
                await _eventProcessor.ProcessEvent(apiContext, eventPayLoad, StatusCode.New);
            }
        }


        public void Deleted(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task DeletedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Updated(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            if (!await ShouldIgnoreCustomer(apiContext, Convert.ToInt32(eventPayLoad.EntityId)))
            {
                var aubEvent = eventPayLoad.ToAubEvent();
                aubEvent.Status = EventStatus.Pending.ToString();
                aubEvent.QueuedDateTime = DateTime.UtcNow;
                await _eventHandler.UpdateEvent(apiContext.TenantId, aubEvent);
            }
        }

        private async Task<bool> ShouldIgnoreCustomer(IApiContext apiContext, int accountId)
        {
            var resource = new CustomerAccountResource(apiContext);
            var account = await resource.GetAccountAsync(accountId, "Id,IsAnonymous,Attributes");

            var filter = String.Format("Id eq '{0}-customeraccount.updated' and Status eq 'Processed'", account.Id);

            var accountEventObj = await _eventHandler.GetEvents(apiContext.TenantId, filter);
            var accountEvent = accountEventObj.FirstOrDefault();

            if (accountEvent != null)
            {
                if (DateTime.Compare(Convert.ToDateTime(accountEvent.ProcessedDateTime), DateTime.Now.AddSeconds(-60)) >
                    0)
                {
                    return true;
                }

            }
            //if (_isLive)
            //{
            //    return account.IsAnonymous || await account.IsEdgeImportCustomer(apiContext);
            //}
            return account.IsAnonymous || account.IsEdgeImportCustomer();
        }
    }
}

using System;
using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Events;
using Mozu.Api.ToolKit.Config;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class ProductEvents : BaseProductEvent, IProductEvents
    {
        public ProductEvents(IAppSetting appSetting) : base(appSetting)
        {
        }
        public void CodeRenamed(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task CodeRenamedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Created(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task CreatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            await ExportProductAsync(apiContext, eventPayLoad);
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
            await ExportProductAsync(apiContext, eventPayLoad);
        }


    }
}

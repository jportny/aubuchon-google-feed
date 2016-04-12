using System;
using System.Threading.Tasks;
using Autofac;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Resources.Platform.Entitylists;
using Mozu.Api.ToolKit.Readers;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Handlers;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class GenericEventProcessor : IEventProcessor
    {
        private readonly IComponentContext _componentContext;
     
        public GenericEventProcessor(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }
        public async Task ProcessEvent(IApiContext apiContext, Event evt, StatusCode statusCode = StatusCode.Active)
        {
            if (!evt.IsApproved()) return;
            var eventProcessor = _componentContext.ResolveNamed<IEventProcessor>(evt.Topic);
            await eventProcessor.ProcessEvent(apiContext, evt, statusCode);
        }
    }
}

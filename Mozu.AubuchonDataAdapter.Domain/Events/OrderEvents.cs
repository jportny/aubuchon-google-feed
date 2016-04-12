using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.Api.Events;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using System;
using System.Threading.Tasks;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public class OrderEvents : IOrderEvents
    {
        private readonly IEventProcessor _eventProcessor;

        public OrderEvents(IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }
        public void Abandoned(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task AbandonedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Cancelled(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task CancelledAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Closed(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task ClosedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Fulfilled(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task FulfilledAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Opened(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public async Task OpenedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            await _eventProcessor.ProcessEvent(apiContext, eventPayLoad, StatusCode.New);
        }

        public void PendingReview(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task PendingReviewAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public void Updated(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }

        public Task UpdatedAsync(IApiContext apiContext, Event eventPayLoad)
        {
            throw new NotImplementedException();
        }
    }
}

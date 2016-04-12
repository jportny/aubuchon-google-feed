using System.Threading.Tasks;
using Mozu.Api;
using Mozu.Api.Contracts.Event;
using Mozu.AubuchonDataAdapter.Domain.Contracts;

namespace Mozu.AubuchonDataAdapter.Domain.Events
{
    public interface IEventProcessor
    {
        Task ProcessEvent(IApiContext apiContext, Event evt, StatusCode statusCode = StatusCode.Active);
    }
}

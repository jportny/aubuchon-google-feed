using System;
using log4net;
using Mozu.Api;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Events;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Quartz;
using ILog = Common.Logging.ILog;
using LogManager = Common.Logging.LogManager;


namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ProcessEventQueueJob : BaseInterruptableJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProcessEventQueueJob));
        private readonly IEventHandler _eventHandler;
        private readonly IEventProcessor _eventProcessor;
        public ProcessEventQueueJob(IEventHandler eventHandler, IEventProcessor eventProcessor, ILogHandler logHandler)
            : base(logHandler)
        {
            _eventHandler = eventHandler;
            _eventProcessor = eventProcessor;
        }
        public override async void ExecuteCore(IJobExecutionContext context)
        {
            try
            {
                ThreadContext.Properties["loggedby"] = "ProcessEventQueueJob";

                var apiContext = new ApiContext(TenantId);
                var filter = String.Format("Status eq '{0}'", EventStatus.Pending);
                var events = await _eventHandler.GetEvents(TenantId, filter);

                foreach (var evt in events)
                {
                    var statusCode = StatusCode.New;
                    if (evt.Topic.Contains(".updated"))
                    {
                        statusCode = StatusCode.Active;
                    }

                    await _eventProcessor.ProcessEvent(apiContext, evt.ToMozuEvent(), statusCode);
                }
                await _eventHandler.PurgeOldEvents(apiContext);
                context.Result = "Completed Sync";
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public override void InterruptCore()
        {

        }
    }
}

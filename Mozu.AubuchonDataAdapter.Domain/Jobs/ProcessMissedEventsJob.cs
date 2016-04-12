using System;
using log4net;
using Mozu.Api;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Quartz;

namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ProcessMissedEventsJob : BaseInterruptableJob
    {
        private readonly IEventHandler _eventHandler;
        private readonly ILog _logger = LogManager.GetLogger(typeof(ProcessMissedEventsJob));
        public ProcessMissedEventsJob(IEventHandler eventHandler, ILogHandler logHandler) : base(logHandler)
        {
            _eventHandler = eventHandler;
        }
        public async override void ExecuteCore(IJobExecutionContext context)
        {
            try
            {
                ThreadContext.Properties["loggedby"] = "ProcessMissedEventsJob";

                var tenantId = int.Parse(context.MergedJobDataMap["tenantId"].ToString());

                var apiContext = new ApiContext(tenantId);

                var lastJobRuntimeUtc = DateTime.UtcNow.AddHours(-3);

                if (context.PreviousFireTimeUtc.HasValue)
                {
                    lastJobRuntimeUtc = context.PreviousFireTimeUtc.Value.DateTime;
                }

                await _eventHandler.ProcessMissedEvents(apiContext, lastJobRuntimeUtc);

            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc);
            }
        }

        public override void InterruptCore()
        {

        }
    }
}

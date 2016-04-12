using System;
using log4net.Appender;
using log4net.Core;
using Mozu.AubuchonDataAdapter.Domain.Contracts;
using Mozu.AubuchonDataAdapter.Domain.Handlers;

namespace Mozu.AubuchonDataAdapter.Domain.Utility
{

    public class LogAppender : AppenderSkeleton
    {
        protected ILogHandler LogHandler;
        protected override async void Append(LoggingEvent loggingEvent)
        {
            var tenantId = loggingEvent.GetProperties()["tenantId"];

            if (tenantId == null) return;

            var logger = loggingEvent.GetProperties()["loggedby"] ?? String.Empty;
            LogHandler = (ILogHandler)loggingEvent.GetProperties()["LogHandler"];

            var log = new AppLog
            {
                Id = Guid.NewGuid(),
                EntityId = String.Empty,
                LoggerName = Convert.ToString(logger),
                LogType = loggingEvent.Level.Name,
                Message = Convert.ToString(loggingEvent.MessageObject),
                StackTrace = String.Empty,
                CreatedOn = DateTime.Now
            };

            if (loggingEvent.ExceptionObject != null)
            {
                log.Message = loggingEvent.ExceptionObject.Message;
                log.StackTrace = loggingEvent.GetExceptionString();
            }
            await LogHandler.Log(Convert.ToInt32(tenantId), log);

        }

    }


    internal class LogProvider
    {
        protected static ILogHandler LogHandler;
        LogProvider(ILogHandler logHandler)
        {
            LogHandler = logHandler;
        }

        public static ILogHandler Logger()
        {
            return LogHandler;
        }
    }

}

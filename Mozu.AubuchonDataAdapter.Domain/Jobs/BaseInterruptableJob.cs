using System;
using System.Configuration;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Quartz;
using log4net;

namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{

    public abstract class BaseInterruptableJob : IInterruptableJob
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(BaseInterruptableJob));

        protected int TenantId;
        public abstract void ExecuteCore(IJobExecutionContext context);

        private readonly ILogHandler _logHandler;
        protected BaseInterruptableJob(ILogHandler logHandler)
        {
            _logHandler = logHandler;
        }


        protected String EdgeFtpHostDirectory { get { return ConfigurationManager.AppSettings["EdgeFTPHostDirectory"]; } }
        protected String EdgeImportTempDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["EdgeImportTempDirectory"];
            }
        }

        protected String ProcessedFolder
        {
            get
            {
                return String.Format("{0}{1}", EdgeImportTempDirectory, "processed");
            }
        }

        protected String FailedFolder
        {
            get
            {
                return String.Format("{0}{1}", EdgeImportTempDirectory, "failed");
            }
        }
        public abstract void InterruptCore();

        public void Interrupt()
        {
            try
            {
                InterruptCore();
            }
            catch (Exception e)
            {
                _log.Error("Error Interrupting Job. " + e.Message);
            }
        }

        public void Execute(IJobExecutionContext context)
        {

            if (context.MergedJobDataMap.Count > 0)
            {
                TenantId = int.Parse(context.MergedJobDataMap["tenantId"].ToString());
            }
            ThreadContext.Properties["tenantId"] = TenantId;
            ThreadContext.Properties["LogHandler"] = _logHandler;

            try
            {
                ExecuteCore(context);
            }
            catch (Exception exception)
            {
                _log.Error("Error executing Job " + context.JobDetail.Key.Name, exception);
            }
        }
    }
}

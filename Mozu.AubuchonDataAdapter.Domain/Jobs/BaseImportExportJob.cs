using log4net;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    public abstract class BaseImportExportJob : IJob
    {
        #region Abstract JobExecute
        public abstract void ExecuteCore(IJobExecutionContext context);
        #endregion

        #region Members
        public readonly ILog Logger = LogManager.GetLogger(typeof(BaseImportExportJob));
        protected IAubuchonLocationHandler LocationHandler;
        #endregion
        
        #region Properties
        public int TenantId { get; set; }
        public string FilePath { get; set; }
        #endregion

        #region Constructor

        protected BaseImportExportJob(IAubuchonLocationHandler locationHandler) 
        {
            LocationHandler = locationHandler;
        }
        #endregion

        #region JobExecute
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                FilePath = context.MergedJobDataMap["file"].ToString();
                TenantId = int.Parse(context.MergedJobDataMap["tenantId"].ToString());

                ExecuteCore(context);
                Logger.Info("Finished Executing...");
            }
            catch (Exception exception)
            {
                Logger.Error("Error executing Job " + context.JobDetail.Key.Name, exception);
            }
        }
        #endregion
    }
}

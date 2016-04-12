using System;
using Common.Logging;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;
namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    public class ImportLocationExtrasJob : BaseImportExportJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ImportLocationExtrasJob));
     
        public ImportLocationExtrasJob(IAubuchonLocationHandler locationHandler) : base(locationHandler) { }

        public override async void ExecuteCore(Quartz.IJobExecutionContext context)
        {
            try
            {
                await
                    LocationHandler.ImportStoreExtras(TenantId, FilePath, Helper.STORE_DETAILS_WS_NAME,
                        Helper.STORE_SERVICES_WS_NAME);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

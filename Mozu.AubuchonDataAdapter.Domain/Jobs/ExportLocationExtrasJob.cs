using System;
using Common.Logging;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    public class ExportLocationExtrasJob : BaseImportExportJob
    {

        private readonly ILog _logger = LogManager.GetLogger(typeof(ExportLocationExtrasJob));
     
        public ExportLocationExtrasJob(IAubuchonLocationHandler locationHandler) : base(locationHandler) { }

        public override async void ExecuteCore(Quartz.IJobExecutionContext context)
        {
            try
            {
                await LocationHandler.ExportAubuchonLocations(TenantId, FilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}

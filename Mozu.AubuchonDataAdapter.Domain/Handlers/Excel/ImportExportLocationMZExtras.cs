using Mozu.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mozu.AubuchonDataAdapter.Domain.Handlers.Excel;
using System.Data;
using Mozu.Api.ToolKit.Config;
using Mozu.Integration.Scheduler;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers.Excel
{
    public interface IImportExportLocationMZExtras 
    {
        string GetImportPath(int? tenantId);
        Task AddImportJobs(int tenantId, string filePath);
        void AddExportJobs(int tenantId, string filePath = null);
    }
    public class ImportExportLocationMZExtras : IImportExportLocationMZExtras
    {
        #region Members
        private readonly IAubuchonLocationHandler _locationHandler;
        private readonly IJobScheduler _jobScheduler;
        #endregion
        
        #region Properties
        private readonly string _fileImportOutputLocation;
        #endregion

        #region Constructor
        public ImportExportLocationMZExtras(IAubuchonLocationHandler locationHandler, IAppSetting appSettings, IJobScheduler scheduler) 
        {
            _locationHandler = locationHandler; 
            _fileImportOutputLocation = appSettings.Settings["ImportExportLocationPath"].ToString();
            _jobScheduler = scheduler;

            System.IO.Directory.CreateDirectory(_fileImportOutputLocation);
        }
        #endregion

        #region AddImportJobs
        public async Task AddImportJobs(int tenantId, string filePath) {

            await _locationHandler.ImportStoreExtras(tenantId, filePath, Helper.STORE_DETAILS_WS_NAME, Helper.STORE_SERVICES_WS_NAME);
        }
        #endregion

        public void AddExportJobs(int tenantId, string outputPath)
        {

            if (String.IsNullOrWhiteSpace(outputPath))
                outputPath = _fileImportOutputLocation;
            var jobParams = new Dictionary<string, object>();
            jobParams.Add("file", outputPath);
            _jobScheduler.AddJob(tenantId, "ExportLocationExtra", typeof(Jobs.ExportLocationExtrasJob), jobParams, null, false, "Export Location Details and Services");
        }

        #region GetImportPath
        public string GetImportPath(int? tenantId) {

            return _fileImportOutputLocation;
        }
        #endregion

    }
}

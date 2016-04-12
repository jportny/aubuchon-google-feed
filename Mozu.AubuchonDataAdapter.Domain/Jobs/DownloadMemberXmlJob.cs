using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Quartz;

namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    [DisallowConcurrentExecution]
    public class DownloadMemberXmlJob : BaseInterruptableJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DownloadMemberXmlJob));
        private readonly IAppSetting _appSetting;

        public DownloadMemberXmlJob(IAppSetting appSetting, ILogHandler logHandler)
            : base(logHandler)
        {
            _appSetting = appSetting;
        }

        public override void ExecuteCore(IJobExecutionContext context)
        {
            var sftpHandler = new SftpHandler(_appSetting);

            var processedFiles = new List<string>(Directory.GetFiles(ProcessedFolder).Select(Path.GetFileName)
                                     .ToArray());
            if (processedFiles.Any()) return;
            var fileNames = new List<string>();
            sftpHandler.GetFiles(EdgeImportTempDirectory
                , EdgeFtpHostDirectory
                , fileNames);
            _logger.Info(String.Format("Downloaded {0} files from SFTP", fileNames.Count));
            context.Result = "Completed";
        }

        public override void InterruptCore()
        {
            throw new NotImplementedException();
        }
    }
}

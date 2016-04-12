using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Common.Logging;
using Mozu.Api.ToolKit.Config;
using Mozu.AubuchonDataAdapter.Domain.Handlers;
using Quartz;

namespace Mozu.AubuchonDataAdapter.Domain.Jobs
{
    [DisallowConcurrentExecution]
    public class SftpUpdateJob :  BaseInterruptableJob
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SftpUpdateJob));
        private readonly IAppSetting _appSetting;

        public SftpUpdateJob(IAppSetting appSetting, ILogHandler logHandler)
            : base(logHandler)
        {
            _appSetting = appSetting;
        }

        public override void ExecuteCore(IJobExecutionContext context)
        {
            var sftpHandler = new SftpHandler(_appSetting);

            var processedFiles = new List<string>(Directory.GetFiles(ProcessedFolder).Select(Path.GetFileName)
                                  .ToArray());

            _logger.Info(String.Format("Deleting {0} processed files", processedFiles.Count));


            sftpHandler.RemoveFiles(processedFiles, EdgeFtpHostDirectory, ProcessedFolder);




            var failedFiles = new List<string>(Directory.GetFiles(FailedFolder).Select(Path.GetFileName)
                                  .ToArray());
            var ftpFailedFolder = String.Format("{0}\failed", EdgeFtpHostDirectory);
            sftpHandler.MoveFiles(failedFiles, EdgeFtpHostDirectory, ftpFailedFolder, FailedFolder);
            context.Result = "Completed";

        }

        public override void InterruptCore()
        {
            throw new NotImplementedException();
        }
    }
}

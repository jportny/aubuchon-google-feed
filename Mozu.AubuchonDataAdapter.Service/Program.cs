using System;
using System.IO;
using Topshelf;
using Mozu.Api.Logging;
using System.Configuration;


namespace Mozu.AubuchonDataAdapter.Service
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(AubuchonService));

        static void Main(string[] args)
        {
            //var filePath = String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory,
            //    "AppSettings.Config");
            //SecureAppSetting.EncryptAppSetting("EdgeFTPPassword", filePath);
            //SecureAppSetting.EncryptAppSetting("EdgeFTPKey", filePath);
            //SecureAppSetting.EncryptAppSetting("EdgeServiceUsername", filePath);
            //SecureAppSetting.EncryptAppSetting("EdgeServicePassword", filePath);

            var serviceName = ConfigurationManager.AppSettings["ServiceName"];
            var displayName = ConfigurationManager.AppSettings["DisplayName"];
            var edgeImportTempDirectory = ConfigurationManager.AppSettings["EdgeImportTempDirectory"];

            try
            {
                if (!Directory.Exists(edgeImportTempDirectory))
                {
                    Directory.CreateDirectory(edgeImportTempDirectory);
                }

                var processedFolder = edgeImportTempDirectory + "processed";
                var failedFolder = edgeImportTempDirectory + "failed";

                if (!Directory.Exists(processedFolder))
                {
                    Directory.CreateDirectory(processedFolder);
                }

                if (!Directory.Exists(failedFolder))
                {
                    Directory.CreateDirectory(failedFolder);
                }

                HostFactory.New(c => c.Service<AubuchonService>(sc =>
                {
                    sc.ConstructUsing(() => new AubuchonService());
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                    c.StartAutomatically();
                    c.RunAsLocalSystem();
                    c.UseLog4Net();
                    c.SetServiceName(serviceName);
                    c.SetDisplayName(displayName);
                    c.SetDescription(displayName);
                })).Run();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }
    }
}

using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Autofac.Core;
using Mozu.Api.ToolKit.Config;
using System;
using System.IO;
using Mozu.AubuchonDataAdapter.Domain.Utility;
using WinSCP;
using System.Collections.Generic;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers
{
    public class SftpHandler
    {

        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _fingerPrint;
        private readonly int _portNumber;


        #region Constructors

        public SftpHandler(IAppSetting appSettings)
        {

            _hostName = (string)appSettings.Settings["EdgeFTPHost"];
            _userName = (string)appSettings.Settings["EdgeFTPUsername"];
            _password = SecureAppSetting.Decrypt((string)appSettings.Settings["EdgeFTPPassword"]);
            _fingerPrint = SecureAppSetting.Decrypt((string)appSettings.Settings["EdgeFTPKey"]);
            _portNumber = Convert.ToInt32(appSettings.Settings["EdgeFTPPort"]);
        }

        public SftpHandler(string hostName, string userName, string password, string fingerprint, int portNumber)
        {
            _hostName = hostName;
            _userName = userName;
            _password = password;
            _fingerPrint = fingerprint;
            _portNumber = portNumber;
        }
        #endregion

        #region Pull
        public bool Pull(string localPath, string remotePath)
        {
            try
            {
                var sessionOptions = GetSessionOptions();

                if (!Directory.Exists(localPath))
                    Directory.CreateDirectory(localPath);

                using (var session = new Session())
                {
                    session.Open(sessionOptions);

                    var result = session.GetFiles(remotePath, localPath, options: new TransferOptions()
                    {
                        TransferMode = TransferMode.Binary,
                        PreserveTimestamp = true
                    });
                    result.Check();
                }
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }


        public void MoveFile(string filePath, string destination)
        {
            var sessionOptions = GetSessionOptions();
            using (var session = new Session())
            {
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                if (session.FileExists(destination))
                {
                    session.RemoveFiles(destination);
                }
                session.MoveFile(filePath, destination);
            }
        }

        public void MoveFiles(IList<String> filesToMove, string remoteSourcePath, string remoteDestinationPath, string localPath)
        {
            var sessionOptions = GetSessionOptions();
            var session = new Session();

            //session.SessionLogPath = "your log path";
            session.Open(sessionOptions); //Attempts to connect to your sFtp site
            //(optional - if different from remote path), Delete source file?, transfer Options  

            Parallel.ForEach(filesToMove, file =>
            {
                var remoteSourceFile = String.Format("{0}/{1}", remoteSourcePath, file);
                var remoteDestFile = String.Format("{0}/{1}", remoteDestinationPath, file);
                var localFile = String.Format(@"{0}\{1}", localPath, file);
                //if (session.FileExists(remoteDestFile))
                //{
                //    session.RemoveFiles(remoteDestFile);
                //}
                if (session.FileExists(remoteSourceFile))
                {
                    session.MoveFile(remoteSourceFile, remoteDestFile);
                }
                File.Delete(localFile);
            });



            //Throw on any error 
            //transferResult.Check();

            //Log information and break out if necessary  
            session.Close();



        }


        public bool GetFile(string destination, string filePath)
        {
            var sessionOptions = GetSessionOptions();
            using (var session = new Session())
            {
                //session.SessionLogPath = "your log path";
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                //Get Ftp File
                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    FilePermissions = null,
                    PreserveTimestamp = false

                };
                //<em style="font-size: 9pt;">null for default permissions.  
                //Can set <em style="font-size: 9pt;">user, Group, or other Read/Write/Execute permissions.  
                //to that of source file - basically change the timestamp to match destination and source files.    
                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                //the parameter list is: remote Path, Local Path with filename 
                //(optional - if different from remote path), Delete source file?, transfer Options  
                var transferResult = session.GetFiles("/" + filePath, destination, false, transferOptions);
                //Throw on any error 
                transferResult.Check();
                //Log information and break out if necessary  
            }
            return true;
        }



        public void GetFiles(string destination, string filePath, List<string> fileNames)
        {
            var session = new Session();

            var sessionOptions = GetSessionOptions();

            session.Open(sessionOptions);
            var transferOptions = new TransferOptions
            {
                TransferMode = TransferMode.Binary,
                FilePermissions = null,
                PreserveTimestamp = false,
                FileMask = @"*.xml|*/"
            };
            //transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

            //var remoteFilesList = session.ListDirectory(filePath);

            var syncResult = session.SynchronizeDirectories(SynchronizationMode.Local, destination, filePath, false,
                false, SynchronizationCriteria.None, transferOptions);
            syncResult.Check();

            var downloadedFiles = new List<string>(Directory.GetFiles(destination).Select(Path.GetFileName)
                                    .ToArray());

            fileNames.AddRange(downloadedFiles);

            //Parallel.ForEach(remoteFilesList.Files, item =>
            //{
            //    if (!item.Name.EndsWith("xml")) return;
            //    var remotePath = String.Format("{0}/{1}", filePath, item.Name);
            //    var destinationPath = String.Format("{0}{1}", destination, item.Name);

            //    var transferResult = session.GetFiles(remotePath, destinationPath, false,
            //        transferOptions);
            //    transferResult.Check();
            //    fileNames.Add(item.Name);

            //});
            session.Dispose();
        }

        public bool GetFilesOnList(string destination, string filePath, List<string> filesList)
        {
            var sessionOptions = GetSessionOptions();
            using (var session = new Session())
            {
                session.Open(sessionOptions);
                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary,
                    FilePermissions = null,
                    PreserveTimestamp = false
                };
                transferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                foreach (var fileName in filesList)
                {
                    var transferResult = session.GetFiles("/" + filePath + "/" + fileName, destination + fileName, false, transferOptions);
                    transferResult.Check();
                }
            }
            return true;
        }

        public List<string> UploadingFileList(string remotePath)
        {
            var sessionOptions = GetSessionOptions();
            var result = new List<string>();

            using (var session = new Session())
            {
                session.Open(sessionOptions);
                var remoteFilesList = session.ListDirectory("/" + remotePath);

                foreach (RemoteFileInfo item in remoteFilesList.Files)
                {
                    if (item.Name.EndsWith("xml"))
                        result.Add(item.Name);
                }
            }
            return result;
        }
        #endregion

        #region Push
        public bool Push(string filePath, string destination)
        {
            try
            {
                // Setup session options
                var sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password,
                    PortNumber = 22,
                    SshHostKeyFingerprint = "ssh-rsa 2048 ed:52:dd:3a:b6:e7:20:58:6f:b5:f1:83:a8:bf:db:5d"
                };

                using (var session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Upload files
                    var transferOptions = new TransferOptions { TransferMode = TransferMode.Binary, PreserveTimestamp = false };
                    var destinationFile = String.Format("{0}{1}", destination, Path.GetFileName(filePath));
                    if (session.FileExists(destinationFile))
                    {
                        var removalResult = session.RemoveFiles(destinationFile);
                    }
                    var transferResult = session.PutFiles(@filePath, destination, true, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    //foreach (TransferEventArgs transfer in transferResult.Transfers)
                    //{
                    //    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    //}
                }

                return true;
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error: {0}", e);
                return false;
            }
        }
        #endregion

        public bool FileExists(string filePath)
        {
            var sessionOptions = GetSessionOptions();
            var result = false;
            using (var session = new Session())
            {
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                result = session.FileExists(filePath);
            }
            return result;
        }

        #region Remove

    
        public void RemoveFiles(IList<String> filesToDelete, string remotePath, string localPath)
        {
           
            var sessionOptions = GetSessionOptions();
            var session = new Session();

            //session.SessionLogPath = "your log path";
            session.Open(sessionOptions); //Attempts to connect to your sFtp site
            //(optional - if different from remote path), Delete source file?, transfer Options  

            Parallel.ForEach(filesToDelete, file =>
            {
                var remoteFile = String.Format("{0}/{1}", remotePath, file);
                var transferResult = session.RemoveFiles(remoteFile);

                var localFile = String.Format(@"{0}\{1}", localPath, file);

                File.Delete(localFile);

            });



            //Throw on any error 
            //transferResult.Check();

            //Log information and break out if necessary  
            session.Close();
          
        }

        public bool RemoveFile(string filePath)
        {
            var result = false;
            var sessionOptions = GetSessionOptions();
            using (var session = new Session())
            {
                //session.SessionLogPath = "your log path";
                session.Open(sessionOptions); //Attempts to connect to your sFtp site
                //(optional - if different from remote path), Delete source file?, transfer Options  
                var transferResult = session.RemoveFiles(filePath);
                result = transferResult.IsSuccess;
                //Throw on any error 
                //transferResult.Check();

                //Log information and break out if necessary  
            }
            return result;
        }
        #endregion

        private SessionOptions GetSessionOptions()
        {
            return new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
                PortNumber = _portNumber,
                SshHostKeyFingerprint = _fingerPrint
            };
        }
    }


}

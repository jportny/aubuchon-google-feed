namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
    public class SftpFileResult
    {
        public string RemotePath { get; set; }
        public string LocalPath { get; set; }
        public string Error { get; set; }
    }
}

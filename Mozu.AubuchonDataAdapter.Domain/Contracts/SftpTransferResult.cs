using System.Collections.Generic;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
    //I'm not sure if I'm going to use this for my job now.
    public class SftpTransferResult 
    {
        public int Count { get; set; }
        public List<SftpFileResult> FileResults { get; set; }
    }
}

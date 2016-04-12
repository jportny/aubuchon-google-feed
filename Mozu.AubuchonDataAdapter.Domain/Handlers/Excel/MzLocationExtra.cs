using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mozu.AubuchonDataAdapter.Domain.Handlers.Excel
{
    public class MzLocationExtra
    {
        public string LocationCode {get;set;}
        public string ManagerName {get; set;}
        public string ManagerImageUrl {get; set;}
        public string StoreFrontImageUrl {get;set;}
        public string BDRName {get;set;}
        public string BDRImageUrl{get;set;}
        public string[] Services { get; set; }
    }
}

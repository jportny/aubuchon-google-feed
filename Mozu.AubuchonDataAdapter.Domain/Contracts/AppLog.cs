using System;
using System.Collections.Generic;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
    public class AppLog
    {
        private string _type;

        public Guid? Id { get; set; }
        public string EntityId { get; set; }
        public string LoggerName { get; set; }
        public string LogType
        {
            get { return _type ?? "ERROR"; }
            set { _type = value; }
        }

        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class Logs
    {
        public IEnumerable<AppLog> Items { get; set; }
        public int? PageCount { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }

        public int? StartIndex { get; set; }
    }
}

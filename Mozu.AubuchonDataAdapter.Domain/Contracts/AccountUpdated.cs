using System;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
    public class AccountUpdated
    {
        public int AccountId { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}

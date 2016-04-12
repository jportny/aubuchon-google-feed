using System;

namespace Mozu.AubuchonDataAdapter.Domain.Contracts
{
    public class AubEvent
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public string EntityId { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }
        public DateTime QueuedDateTime { get; set; }
        public DateTime ProcessedDateTime { get; set; }

        public override string ToString()
        {
            return String.Format("{0} | {1}~{2}~{3}~{4}", Id, EntityId, Topic, Status, QueuedDateTime);
        }

    }

    public enum EventStatus
    {
        Processed,
        Pending,
        Failed
    }


}

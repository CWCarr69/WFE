using System;

namespace Timesheet.Domain.Models.Purge
{
    public class PurgeOldData : Entity
  {
        public string Id { get; set; }
        public DateTime PurgeDate { get; set; }
        public DateTime OlderThan { get; set; }
        public int RecordsPurged { get; set; }
        public string? PerformedBy { get; set; }

        public PurgeOldData(string id, DateTime olderThan, int recordsPurged, string? performedBy):base(id)
    {
            PurgeDate = DateTime.UtcNow;
            OlderThan = olderThan;
            RecordsPurged = recordsPurged;
            PerformedBy = performedBy;
        }
    }
}
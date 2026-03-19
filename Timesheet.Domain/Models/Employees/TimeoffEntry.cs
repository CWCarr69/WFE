using System;

namespace Timesheet.Domain.Models.Employees
{
    public class TimeoffEntry : Entity
    {
        public TimeoffEntry(string id, DateTime requestDate, int typeId, double hours, string label, string? profitCenter = null) : base(id)
        {
            RequestDate = requestDate;
            TypeId = typeId;
            Hours = hours;
            Label = label;
            ProfitCenter = profitCenter;
        }

        public static TimeoffEntry Create(DateTime requestDate, int typeId, double hours, string label, string? profitCenter = null)
        {
            var entry = new TimeoffEntry(Guid.NewGuid().ToString(), requestDate.Date, typeId, hours, label, profitCenter);
            entry.Status = TimeoffEntryStatus.NOT_PROCESSED;

            return entry;
        }

        public DateTime RequestDate { get; private set; }
        public int TypeId { get; private set; }
        public double Hours { get; private set; }
        public TimeoffEntryStatus Status { get; private set; }
        public string? Label { get; private set; }
        public string? ProfitCenter { get; private set; }

        internal void Validate()
        {
            this.Status = TimeoffEntryStatus.PROCESSED;
            this.UpdateMetadata();
        }

        internal void Reject()
        {
            this.Status = TimeoffEntryStatus.REJECTED;
            this.UpdateMetadata();
        }

        internal void Update(int typeId, double hours, string label, string? profitCenter = null)
        {
            this.TypeId = typeId;
            this.Label = label ?? this.Label;
            this.Hours = hours != 0 ? hours : this.Hours;
            this.ProfitCenter = profitCenter ?? this.ProfitCenter;
            this.UpdateMetadata();
        }
    }
}
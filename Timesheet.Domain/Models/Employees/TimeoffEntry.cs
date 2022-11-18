namespace Timesheet.Domain.Models.Employees
{
    public class TimeoffEntry : Entity
    {
        public TimeoffEntry(string id, DateTime requestDate, TimeoffType type, double hours, string label) : base(id)
        {
            RequestDate = requestDate;
            Type = type;
            Hours = hours;
            Label = label;
        }

        public static TimeoffEntry Create(DateTime requestDate, TimeoffType type, double hours, string label)
        {
            var entry = new TimeoffEntry(Guid.NewGuid().ToString(), requestDate.Date, type, hours, label);
            entry.Status = TimeoffEntryStatus.NOT_PROCESSED;

            return entry;
        }

        public DateTime RequestDate { get; private set; }
        public TimeoffType Type { get; private set; }
        public double Hours { get; private set; }
        public TimeoffEntryStatus Status { get; private set; }
        public string Label { get; private set; }

        internal void Validate()
        {
            this.Status = TimeoffEntryStatus.PROCESSED;
        }

        internal void Update(TimeoffType type, double hours, string label)
        {
            this.Type = type;
            this.Label = label ?? this.Label;
            this.Hours = hours != 0 ? hours : this.Hours;
        }
    }
}
namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetHoliday : Entity
    {
        private const int DefaultHolidayHours = 8;

        public TimesheetHoliday(string id) : base(id)
        {
        }

        public TimesheetHoliday(string id, DateTime WorkDate, string description) : base(id)
        {
            this.WorkDate = WorkDate.Date;
            PayrollCode = "HOLIDAY";
            Hours = DefaultHolidayHours;
            Description = description ?? string.Empty;
            Status = TimesheetEntryStatus.APPROVED;
        }

        public DateTime WorkDate { get; private set; }
        public string PayrollCode { get; private set; }
        public double Hours { get; private set; }
        public double Quantity => Hours;
        public string Description { get; private set; }

        public TimesheetEntryStatus Status { get; private set; }

        internal void Updated(string description)
        {
            this.Description = description ?? this.Description;
        }
    }
}

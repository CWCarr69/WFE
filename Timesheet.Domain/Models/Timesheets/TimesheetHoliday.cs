using Timesheet.Models.Referential;

namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetHoliday : Entity
    {
        private const int DefaultHolidayHours = 8;

        public static TimesheetHoliday Create(string holidayId, DateTime WorkDate, string description)
        => new TimesheetHoliday(GenerateId(), holidayId, WorkDate, description);

        public TimesheetHoliday(string id) : base(id)
        {
        }

        public TimesheetHoliday(string id, string holidayId, DateTime WorkDate, string description) 
            : base(id)
        {
            this.HolidayId = holidayId;
            this.WorkDate = WorkDate.Date;
            PayrollCodeId = (int)TimesheetFixedPayrollCodeEnum.HOLIDAY;
            Hours = DefaultHolidayHours;
            Description = description ?? string.Empty;
            Status = TimesheetEntryStatus.APPROVED;
        }

        public string HolidayId { get; private set; }

        public DateTime WorkDate { get; private set; }
        public int PayrollCodeId { get; private set; }
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

namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetException : Entity
    {
        public TimesheetException(string id) : base(id)
        {
        }

        public TimesheetException(string timeId, string employeeId, string type) : this(GenerateId())
        {
            TimesheetEntryId = timeId;
            EmployeeId = employeeId;
            Type = type;
        }

        public string TimesheetEntryId { get; set; }
        public string EmployeeId { get; set; }
        public string Type { get; set; }
    }
}

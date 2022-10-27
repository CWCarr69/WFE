namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetComment : Entity
    {
        public TimesheetComment(string id) : base(id)
        {
        }

        public TimesheetComment(string employeeId, string timehseetId)
            :base(GenerateId())
        {
            EmployeeId = employeeId;
            TimesheetId = timehseetId;
        }

        public string EmployeeId { get; set; }
        public string TimesheetId { get; set; }
        public string EmployeeComment { get; set; }
        public string ApproverComment { get; set; }

        public void UpdateApproverComment(string? comment)
        {
            ApproverComment = comment ?? ApproverComment;
        }

        public void UpdateEmployeeComment(string? comment)
        {
            EmployeeComment = comment ?? EmployeeComment;
        }
    }
}

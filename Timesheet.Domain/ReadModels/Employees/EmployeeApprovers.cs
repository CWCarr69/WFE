namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeApprovers
    {
        public string EmployeeId { get; set; }
        public string primaryApproverId { get; set; }
        public string primaryApproverFullName { get; set; }
        public string secondaryApproverId { get; set; }
        public string secondaryApproverFullName { get; set; }
    }
}

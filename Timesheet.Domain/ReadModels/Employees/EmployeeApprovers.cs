namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeApprovers
    {
        public string EmployeeId { get; set; }
        public string PrimaryApproverId { get; set; }
        public string PrimaryApproverFullName { get; set; }
        public string SecondaryApproverId { get; set; }
        public string SecondaryApproverFullName { get; set; }
    }
}

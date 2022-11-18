using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoff
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimeoffId { get; set; }
        public double TotalHours { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime RequestStartDate { get; set; }
        public DateTime RequestEndDate { get; set; }
        public TimeoffStatus Status { get; set; }
        public string EmployeeComment { get; set; }
        public string ApproverComment { get; set; }
        public string StatusName => Status.ToString();
        public IEnumerable<EmployeeTimeoffEntry> Entries { get; set; } = Enumerable.Empty<EmployeeTimeoffEntry>();
    }
}

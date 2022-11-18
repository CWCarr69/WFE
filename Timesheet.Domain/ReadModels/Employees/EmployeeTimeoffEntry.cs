using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoffEntry
    {
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime RequestDate { get; set; }
        public TimeoffType Type { get; set; }
        public string PayrollCode => Type.ToString();
        public double Hours { get; set; }
        public string Label { get; set; }
        public string TimeoffHeaderId { get; set; }
    }
}

using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoffDetailSummary
    {
        public DateTime RequestDate { get; set; }
        public TimeoffType Type { get; set; }
        public string PayrollCode => Type.ToString();
        public double Hours { get; set; }
    }
}

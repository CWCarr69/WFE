using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoffDetailSummary
    {
        public DateTime RequestDate { get; set; }
        public int TypeId { get; set; }
        public string PayrollCode { get; set; }
        public double Hours { get; set; }
    }
}

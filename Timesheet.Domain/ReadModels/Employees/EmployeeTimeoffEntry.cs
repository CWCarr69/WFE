using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoffEntry
    {
        public string Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime RequestDate { get; set; }
        public int TypeId { get; set; }
        public string PayrollCode { get; set; }
        public double Hours { get; set; }
        public string Label { get; set; }
        public string TimeoffHeaderId { get; set; }
    }
}

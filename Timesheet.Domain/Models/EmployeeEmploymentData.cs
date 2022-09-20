namespace Timesheet.Domain.Models
{
    public class EmployeeEmploymentData
    {
        public string JobTitle { get; set; }
        public Employee Supervisor { get; set; }
        public Employee Manager { get; set; }
        public string Department { get; set; }
        public DateTime EmploymentDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public decimal UnitCost { get; set; }
        public bool IsSalaried { get; set; }
        public int YearsInService { get; set; }
    }
}

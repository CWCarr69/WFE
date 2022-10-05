namespace Timesheet.Domain.Models.Employees
{
    public class EmployeeEmploymentData
    {
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public DateTime EmploymentDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public decimal UnitCost { get; set; }
        public bool IsSalaried { get; set; }
        public int YearsInService { get; set; }
    }
}

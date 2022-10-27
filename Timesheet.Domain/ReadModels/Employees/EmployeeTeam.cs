namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTeam
    {
        public int TotalItems { get; set; }
        public IEnumerable<EmployeeWithTimeStatus> Employees {get; set;}
    }
}

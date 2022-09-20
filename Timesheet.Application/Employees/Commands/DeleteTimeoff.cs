namespace Timesheet.Application.Employees.Commands
{
    public class DeleteTimeoff : ICommand
    {
       public string EmployeeId { get; set; }
       public string TimeoffId { get; set; }
    }
}

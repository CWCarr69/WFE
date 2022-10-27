namespace Timesheet.Domain.Models.Employees
{
    public class User
    {
        public string Id { get; set; }
        public bool IsAdministrator { get; set; } = false;
    }
}

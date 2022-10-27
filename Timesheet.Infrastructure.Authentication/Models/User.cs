using Timesheet.Domain.Models.Employees;

namespace Timesheet.Infrastructure.Authentication.Models
{
    public class User
    {
        public string Id { get; internal set; }
        public string Fullname { get; internal set; }
        public string Login { get; internal set; }
        public bool IsAdministrator { get; internal set; }
        public EmployeeRole Role { get; internal set; }
    }
}
using Timesheet.Domain.Models;

namespace Timesheet.ReadModel.ReadModels
{
    public class EmployeeWithTimeStatus
    {
        public EmployeeWithTimeStatus(Employee employee)
        {
            Id = employee.Id;
            FullName = employee.FullName;
            LastTimeoffStatus = employee.LastTimeoffStatus()?.ToString() ?? "None";
        }

        public string Id { get; set; }
        public string FullName { get; set; }
        public string LastTimeoffStatus { get; set; }
        public string LastTimesheetStatus { get; set; }

        public static explicit operator EmployeeWithTimeStatus(Employee e)
            => new EmployeeWithTimeStatus(e);
    }
}

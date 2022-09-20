using Timesheet.Domain.Models;

namespace Timesheet.ReadModel.ReadModels
{
    public class EmployeePendingTimeoff
    {
        public EmployeePendingTimeoff(Timeoff timeoff)
        {
            TimeoffId = timeoff.Id;
            TotalHours = timeoff.TimeoffEntries.Sum(t => t.Hours);
        }

        public string TimeoffId { get; set; }
        public double TotalHours { get; set; }
        public DateTime CreatedDate { get; set; }

        public static explicit operator EmployeePendingTimeoff(Timeoff e)
            => new EmployeePendingTimeoff(e);
    }

    public class EmployeeWithPendingTimeoffs
    {
        public EmployeeWithPendingTimeoffs(Employee employee)
        {
            Id = employee.Id;
            Fullname = employee.FullName;
            PendingTimeoffs = employee.Timeoffs.Select(timeoff => (EmployeePendingTimeoff) timeoff);
        }

        public string Id { get; set; }
        public string Fullname { get; set; }
        public IEnumerable<EmployeePendingTimeoff> PendingTimeoffs { get; set; }

        public static explicit operator EmployeeWithPendingTimeoffs(Employee e)
            => new EmployeeWithPendingTimeoffs(e);
    }
}

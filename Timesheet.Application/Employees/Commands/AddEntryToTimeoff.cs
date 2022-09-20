using Timesheet.Domain.Models;

namespace Timesheet.Application.Employees.Commands
{
    public class AddEntryToTimeoff : ICommand
    {
        public string EmployeeId { get; set; }
        public string TimeoffId { get; set; }
        public DateTime RequestDate { get; set; }
        public TimeoffType Type { get; set; }
        public double Hours { get; set; }
    }
}

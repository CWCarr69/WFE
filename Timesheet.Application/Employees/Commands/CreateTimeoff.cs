namespace Timesheet.Application.Employees.Commands
{
    public class CreateTimeoff : ICommand
    {
        public DateTime RequestStartDate { get; set; }
        public DateTime RequestEndDate { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeComment { get; set; }
        public string SupervisorComment { get; set; }

        public IEnumerable<AddEntryToTimeoff> Entries { get; set; }
    }
}

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoff
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimeoffId { get; set; }
        public double TotalHours { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime RequestStartDate { get; set; }
        public DateTime RequestEnddDate { get; set; }
        public string Status { get; set; }
    }
}

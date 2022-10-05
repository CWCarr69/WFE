namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoffDetail
    {
        public string TimeoffId { get; set; }
        public double TotalHours { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime RequestDate { get; private set; }
        public string PayrollCode { get; private set; }
        public double Hours { get; private set; }
    }
}

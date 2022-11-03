using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeTimeoffMonthStatistics
    {
        public DateTime Month { get; set; }
        public TimeoffStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public double Hours { get; set; }
    }

    public class EmployeeTimeoffMonthStatisticsGroupByMonth
    {
        public DateTime Month { get; set; }
        public IEnumerable<EmployeeTimeoffMonthStatistics> MonthStatistics { get; set; }
    }

}

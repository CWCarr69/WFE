using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Employees.Services
{
    public class EmployeeTimeoffMonthStatisticsServices
    {

        public EmployeeMonthStatistics CalculateCurrentMonthStatistics(in Employee employee)
        {
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            var timeoffs = employee.Timeoffs.Where(t =>
            {
                return t.RequestStartDate.Month == currentMonth && t.RequestEndDate.Month == currentMonth
                && t.RequestStartDate.Year == currentYear && t.RequestEndDate.Year == currentYear;
            });

            Func<TimeoffHeader, double?> SumHours = timeoff => timeoff.TimeoffEntries?.Sum(e => e.Hours);

            var statictics = new EmployeeMonthStatistics
            {
                TotalHours = timeoffs.Sum(SumHours).GetValueOrDefault(),
                TotalInProgress = timeoffs.Where(t => t.Status.Equals(TimeoffStatus.IN_PROGRESS)).Sum(SumHours).GetValueOrDefault(),
                TotalSubmitted = timeoffs.Where(t => t.Status.Equals(TimeoffStatus.SUBMITTED)).Sum(SumHours).GetValueOrDefault(),
                TotalApproved = timeoffs.Where(t => t.Status.Equals(TimeoffStatus.APPROVED)).Sum(SumHours).GetValueOrDefault(),
                TotalRejected = timeoffs.Where(t => t.Status.Equals(TimeoffStatus.REJECTED)).Sum(SumHours).GetValueOrDefault()
            };

            return statictics;
        }
        
    }

    public class EmployeeMonthStatistics
    {

        public double TotalHours { get; set; }
        public double TotalSubmitted { get; set; }
        public double TotalInProgress { get; set; }
        public double TotalRejected { get; set; }
        public double TotalApproved { get; set; }

    }
}

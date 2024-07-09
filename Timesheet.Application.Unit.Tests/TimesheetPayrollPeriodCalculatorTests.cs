using Timesheet.Domain.Employees.Services;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Unit.Tests
{
    public class TimesheetPayrollPeriodCalculatorTests
    {
        [Theory]
        [InlineData("2023-12-14", "2023-51H")]
        [InlineData("2023-12-21", "2023-52H")]
        [InlineData("2023-12-28", "2024-1H")]
        [InlineData("2024-01-04", "2024-2H")]
        [InlineData("2024-01-11", "2024-3H")]
        public void ShouldCalculateCorrectPayrollPeriod(string nowString, string expectedPayrollPeriod)
        {
            var now = DateTime.Parse(nowString);

            Assert.Equal(TimesheetHeader.CreateWeeklyTimesheet(now).PayrollPeriod, expectedPayrollPeriod);
        }
    }
}
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Domain.Unit.Tests
{
    public class TimesheetTests
    {
        [Theory]
        [InlineData(2023, 4, 29, "2023-19H")]
        [InlineData(2022, 12, 23, "2023-1H")]
        [InlineData(2023, 1, 1, "2023-2H")]
        [InlineData(2023, 6, 2, "2023-24H")]
        [InlineData(2023, 12, 22, "2023-53H")]
        public void ShouldGenerateCorrectCodeForWeeklyTimesheet(int year, int month, int day, string code)
        {
            var now = new DateTime(year, month, day);
            var timesheet = TimesheetHeader.CreateWeeklyTimesheet(now);
            Assert.Equal(code, timesheet.Id);
        }

        [Theory]
        [InlineData(2023, 4, 29, "2023-8SS")]
        [InlineData(2023, 1, 1, "2023-1SS")]
        public void ShouldGenerateCorrectCodeForMonthlyTimesheet(int year, int month, int day, string code)
        {
            var now = new DateTime(year, month, day);
            var timesheet = TimesheetHeader.CreateMonthlyTimesheet(now);
            Assert.Equal(code, timesheet.Id);
        }
    }
}
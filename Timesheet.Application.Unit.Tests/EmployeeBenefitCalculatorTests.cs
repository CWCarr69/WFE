using Timesheet.Domain.Employees.Services;

namespace Timesheet.Application.Unit.Tests
{
    public class EmployeeBenefitCalculatorTests
    {
        [Theory]
        [InlineData("1994-06-12","2023-09-25")]
        public void ShouldCalculateCorrectVaccation(string employmentDateString, string nowString)
        {
            var employmentDate = DateTime.Parse(employmentDateString);
            var now = DateTime.Parse(nowString);

            Assert.Equal(new EmployeeBenefitCalculator().GetTotalCurrentVacationsTime(employmentDate, now, 0), 160);
        }
    }
}
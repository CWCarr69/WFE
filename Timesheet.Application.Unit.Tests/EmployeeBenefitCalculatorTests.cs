using Timesheet.Domain.Employees.Services;

namespace Timesheet.Application.Unit.Tests
{
    public class EmployeeBenefitCalculatorTests
    {
        [Theory]
        [InlineData("2022-05-4","2023-09-25")]
        public void ShouldCalculateCorrectVaccation(string employmentDateString, string nowString)
        {
            var employmentDate = DateTime.Parse(employmentDateString);
            var now = DateTime.Parse(nowString);

            Assert.Equal(new EmployeeBenefitCalculator().GetTotalCurrentVacationsTime(employmentDate, now, 0), 0);
        }
    }
}
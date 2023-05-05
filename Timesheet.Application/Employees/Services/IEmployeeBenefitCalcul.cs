using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Application.Employees.Services
{
    public interface IEmployeeBenefitCalculator
    {
        Task<EmployeeCalculatedBenefits> GetBenefits(string employeeId, DateTime employmentDate, int cumulatedPreviousWorkPeriod);
    } 
}
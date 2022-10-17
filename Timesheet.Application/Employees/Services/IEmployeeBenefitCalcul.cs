using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Domain.Employees.Services
{
    public interface IEmployeeBenefitCalculator
    {
        Task<EmployeeBenefits> GetBenefits(string employeeId, DateTime value);
    } 
}
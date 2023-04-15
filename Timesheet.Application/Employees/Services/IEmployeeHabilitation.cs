
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Employees.Services
{
    public interface IEmployeeHabilitation
    {
        Task<EmployeeRoleOnData> GetEmployeeRoleOnData(string author, string dataOwner, bool isAdministrator);
    }
}

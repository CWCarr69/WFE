
using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Employees.Services
{
    public interface IEmployeeHabilitation
    {
        EmployeeRoleOnData GetEmployeeRoleOnData(
            string authorId,
            bool isAdministrator,
            string dataOwnerId,
            string? dataOwnerPrimaryApprover,
            string? dataOwnerSecondaryAPprover);
    }
}

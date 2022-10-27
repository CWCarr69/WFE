using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Employees.Services
{
    public class EmployeeHabilitation : IEmployeeHabilitation
    {
        public EmployeeRoleOnData GetEmployeeRoleOnData(string author, bool isAdministrator, string dataOwner, string? dataOwnerPrimaryApprover, string? dataOwnerSecondaryApprover)
        {
            var employeeRoleOnData = EmployeeRoleOnData.NONE;
            if (isAdministrator)
            {
                employeeRoleOnData = EmployeeRoleOnData.ADMINISTRATOR;
            }
            else if (author is not null && author == dataOwner)
            {
                employeeRoleOnData = EmployeeRoleOnData.CREATOR;
            }
            else if (author is not null && author == dataOwnerPrimaryApprover)
            {
                employeeRoleOnData = EmployeeRoleOnData.APPROVER;
            }
            else if (author is not null && author == dataOwnerSecondaryApprover)
            {
                employeeRoleOnData = EmployeeRoleOnData.APPROVER;
            }

            return employeeRoleOnData;
        }
    }
}

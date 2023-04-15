using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Employees.Services
{
    public class EmployeeHabilitation : IEmployeeHabilitation
    {
        private readonly IHierarchyRepository _hierarchy;

        public EmployeeHabilitation(IHierarchyRepository hierarchy)
        {
            this._hierarchy = hierarchy;
        }

        public async Task<EmployeeRoleOnData> GetEmployeeRoleOnData(string author, string dataOwner, bool isAdministrator)
        {
            if(author is null)
            {
                return EmployeeRoleOnData.NONE;
            }


            if (isAdministrator)
            {
                return EmployeeRoleOnData.ADMINISTRATOR;
            }

            if (author == dataOwner)
            {
                return EmployeeRoleOnData.CREATOR;
            }

            if (await _hierarchy.IsEmployeeManager(dataOwner, author))
            {
                return EmployeeRoleOnData.APPROVER;
            }

            return EmployeeRoleOnData.NONE;
        }
    }
}

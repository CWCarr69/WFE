using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    public class Transition {
        public Transition(Enum name, params Enum[] permissionStates)
        {
            Name = name;
            PermissionStates = permissionStates;
        }

        public Transition AuthorizeRoles(params EmployeeRoleOnData[] authorizedRoles)
        {
            AuthorizedRoles = authorizedRoles;
            return this;
        }

        public Enum Name { get; }
        public EmployeeRoleOnData[] AuthorizedRoles { get; private set; }
        public Enum[] PermissionStates { get; }
    }
}

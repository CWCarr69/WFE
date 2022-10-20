using Timesheet.Domain.Models.Employees;

namespace Timesheet.Application.Workflow
{
    public class Transition {
        public Transition(Enum name, params Enum[] permissionStates)
        {
            Name = name;
            PermissionStates = permissionStates;
        }

        public Enum Name { get; }
        public EmployeeRoleOnData[] AuthorizedRoles { get; }
        public Enum[] PermissionStates { get; }
    }
}

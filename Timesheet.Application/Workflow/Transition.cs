namespace Timesheet.Application.Workflow
{
    internal record Transition(Enum Name, params Enum[] PermissionStates);
}

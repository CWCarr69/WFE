namespace Timesheet.Application.Workflow
{
    public record Transition(Enum Name, params Enum[] PermissionStates);
}

namespace Timesheet.Application.Shared
{
    public abstract class BaseCommand : ICommand
    {
        public string? AuthorId { get; set; }

        public abstract CommandActionType ActionType();
       
    }
}

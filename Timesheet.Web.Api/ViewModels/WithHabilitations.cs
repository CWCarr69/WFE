namespace Timesheet.Web.Api.ViewModels
{
    public record AuthorizedAction(int Value, String Name);

    public class WithHabilitations<T>
    {
        public WithHabilitations(T data, List<AuthorizedAction> authorizedActions = null)
        {
            Data = data;
            AuthorizedActions = authorizedActions;
        }

        public T Data { get; set; }
        public List<AuthorizedAction> AuthorizedActions { get; set; } = new();
    }
}

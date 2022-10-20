namespace Timesheet.Web.Api.ViewModels
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public IEnumerable<NotificationPopulationDto> Populations { get; set; }
    }
}

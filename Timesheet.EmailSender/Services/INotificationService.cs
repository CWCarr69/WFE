namespace Timesheet.EmailSender.Services
{
    public interface INotificationService
    {
        public void SendNotifications();
        public void SendTestNotifications();
    }
}

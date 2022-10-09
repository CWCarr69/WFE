using Microsoft.Extensions.DependencyInjection;
using Timesheet.EmailSender.Services;

namespace Timesheet.EmailSender
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection().AddServices().BuildServiceProvider();
            var notificationService = serviceProvider.GetService<INotificationService>();
            notificationService.SendNotifications();
        }
    }
}
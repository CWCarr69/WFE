using Timesheet.Domain.Models.Notifications;

namespace Timesheet.Domain.Exceptions
{
    public sealed class InvalidNotificationPopulationException : DomainException
    {
        public InvalidNotificationPopulationException(int population) 
        : base($"{nameof(Notification)}.InvalidPopulation", 400, $"{nameof(Notification)} cannot be updated with population {population}.")
        {
        }
    }
}

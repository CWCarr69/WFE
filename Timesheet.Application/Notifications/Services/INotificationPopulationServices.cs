using Timesheet.Domain.Models.Notifications;

namespace Timesheet.Application.Notifications.Services
{
    public interface INotificationPopulationServices
    {
        int Construct(IEnumerable<NotificationPopulationType> populations);
        IEnumerable<NotificationPopulationType> Deconstruct(int population);
        IDictionary<NotificationPopulationType, string> MatchPopulations((string Id, string PrimaryApproverId, string SecondaryApproverId) employee);
        Task AddAdministratorsToEmployeesIfConcerned(IEnumerable<NotificationPopulationType> populationsConcerned, List<string> employeeIds);
    }
}

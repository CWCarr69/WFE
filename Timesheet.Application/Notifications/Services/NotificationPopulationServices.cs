using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Notifications.Services
{
    public class NotificationPopulationServices : INotificationPopulationServices
    {
        private readonly IEmployeeReadRepository _employeeReadRepository;

        public NotificationPopulationServices(IEmployeeReadRepository employeeReadRepository)
        {
            this._employeeReadRepository = employeeReadRepository;
        }

        public readonly int[] AllPopulation = new int[]
        {
            (int) NotificationPopulationType.EMPLOYEE,
            (int) NotificationPopulationType.PRIMARY_APPROVER,
            (int) NotificationPopulationType.SECONDARY_APPROVER,
            (int) NotificationPopulationType.ADMINISTRATOR,
        };

        public int Construct(IEnumerable<NotificationPopulationType> populations)
        {
            return populations.Sum(p => (int)p);
        }

        public IEnumerable<NotificationPopulationType> Deconstruct(int population)
        {
            var populationsConcerned = new List<NotificationPopulationType>();
            foreach (var referencePopulation in AllPopulation)
            {
                var populationConcerned = referencePopulation | population;
                if (populationConcerned != 0)
                {
                    populationsConcerned.Add((NotificationPopulationType)populationConcerned);
                }
            }

            return populationsConcerned;
        }

        public IDictionary<NotificationPopulationType, string> MatchPopulations((string Id, string PrimaryApproverId , string SecondaryApproverId) employee)
        {
            return new Dictionary<NotificationPopulationType, string>() {
                { NotificationPopulationType.EMPLOYEE, employee.Id },
                { NotificationPopulationType.PRIMARY_APPROVER, employee.PrimaryApproverId },
                { NotificationPopulationType.SECONDARY_APPROVER, employee.SecondaryApproverId }
            };
        }

        public async Task AddAdministratorsToEmployeesIfConcerned(IEnumerable<NotificationPopulationType> populationsConcerned, List<string> employeeIds)
        {
            if (populationsConcerned.Any(p => p == NotificationPopulationType.ADMINISTRATOR))
            {
                var administrators = await GetAdministratorIds();
                employeeIds.AddRange(administrators);
            }
        }

        //TODO
        private async Task<IEnumerable<string>> GetAdministratorIds()
        {
            return (await _employeeReadRepository.GetAdministrators())
                .Select(p => p.Id);
        }
    }
}

using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Employee;

namespace Timesheet.Application.Notifications
{
    public class TimeoffWorkflowEventHandler : IEventHandler<TimeoffWorkflowChanged>
    {
        private readonly INotificationReadRepository _readRepository;
        private readonly IWriteRepository<NotificationItem> _writeRepository;
        private readonly IUnitOfWork _unitOfWork;

        private int[] AllPopulation {
            get => new int[]
            {
                (int) NotificationPopulationType.EMPLOYEE,
                (int) NotificationPopulationType.PRIMARY_APPROVER,
                (int) NotificationPopulationType.SECONDARY_APPROVER,
                (int) NotificationPopulationType.ADMINISTRATOR,
            };
        }

        public TimeoffWorkflowEventHandler(
            INotificationReadRepository readRepository,
            IWriteRepository<NotificationItem> writeRepository,
            IUnitOfWork unitOfWork)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task Handle(TimeoffWorkflowChanged @event)
        {
            var notification = _readRepository.GetByGroupAndAction(NotificationType.TIMEOFF, @event.Action);
            if(notification is null)
            {
                return;
            }

            var notificationItems = GenerateNotificationItems(notification, @event,
                new Dictionary<NotificationPopulationType, string>() {
                    {NotificationPopulationType.EMPLOYEE, @event.EmployeeId},
                    {NotificationPopulationType.PRIMARY_APPROVER, @event.SupervisorId},
                    {NotificationPopulationType.SECONDARY_APPROVER, @event.SupervisorId},
                }
            );

            foreach(var item in notificationItems)
            {
                _writeRepository.Add(item);
            }

            await _unitOfWork.CompleteAsync(CancellationToken.None);
        }

        private IEnumerable<NotificationItem> GenerateNotificationItems(Notification notification, TimeoffWorkflowChanged @event, IDictionary<NotificationPopulationType, string> employees)
        {
            List<NotificationPopulationType> populationsConcerned = ExtractEmployeesFromPopulation(notification);

            var employeeIds = populationsConcerned.Select(p => employees[p]).ToList();
            if(populationsConcerned.Any(p => p == NotificationPopulationType.ADMINISTRATOR))
            {
                employeeIds.AddRange(GetAdministrators());
            }

            var employeesConcerned = populationsConcerned.Select(p => 
                NotificationItem.Create(employees[p], notification.Action, ActionToSubject(notification.Action), false, @event.ObjectId));

            return employeesConcerned;
        }

        private string ActionToSubject(string action)
        {
            if (action == TimeoffStatus.REJECTED.ToString())
            {
                return "Timeoff request is rejected";
            }

            if (action == TimeoffStatus.SUBMITTED.ToString())
            {
                return "Timeoff request is submitted";
            }

            if (action == TimeoffStatus.IN_PROGRESS.ToString())
            {
                return "Timeoff request is created";
            }

            if (action == TimeoffStatus.APPROVED.ToString())
            {
                return "Timeoff request is approved";
            }

            throw new InvalidOperationException($"Not an adequate action for the timeoff workflow Handler action {action}");
        }

        private List<NotificationPopulationType> ExtractEmployeesFromPopulation(Notification notification)
        {
            var populationsConcerned = new List<NotificationPopulationType>();
            foreach (var population in AllPopulation)
            {
                var populationConcerned = notification.Population | population;
                if (populationConcerned == 0)
                {
                    populationsConcerned.Add((NotificationPopulationType)populationConcerned);
                }
            }

            return populationsConcerned;
        }

        //TODO
        private IEnumerable<string> GetAdministrators()
        {
            throw new NotImplementedException();
        }
    }
}

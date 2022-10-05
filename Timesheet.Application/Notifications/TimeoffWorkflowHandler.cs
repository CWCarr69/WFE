using Timesheet.Domain.Models;
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
            var notification = _readRepository.GetByGroupAndAction(SubDomainType.TIMEOFF, @event.Action);
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
            var populationsConcerned = new List<NotificationPopulationType>();
            foreach(var population in AllPopulation)
            {
                var populationConcerned = notification.Population | population;
                if(populationConcerned == 0)
                {
                    populationsConcerned.Add((NotificationPopulationType)populationConcerned);
                }
            }

            var employeesConcerned = populationsConcerned.Select(p => NotificationItem.Create(employees[p], notification.Action, false, @event.ObjectId));

            return employeesConcerned;
        }
    }
}

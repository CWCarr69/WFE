using Timesheet.Application.Notifications.Services;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Notifications.EventHandlers
{
    public abstract class BaseTimeStateChangedEventHandler
    {
        private readonly INotificationReadRepository _readRepository;
        private readonly IWriteRepository<NotificationItem> _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationPopulationServices _populationServices;


        protected BaseTimeStateChangedEventHandler(
            INotificationPopulationServices populationServices,
            INotificationReadRepository readRepository,
            IWriteRepository<NotificationItem> writeRepository,
            IUnitOfWork unitOfWork)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
            this._unitOfWork = unitOfWork;
            this._populationServices = populationServices;
        }

        protected async Task Handle(NotificationType type, string employeeId, string primaryApproverId, string secondaryApproverId, string transition, string objectId)
        {
            var notification = _readRepository.GetByGroupAndAction(type, transition);
            if (notification is null)
            {
                return;
            }

            var employee = (employeeId, primaryApproverId, secondaryApproverId);
            var notificationItems = GenerateNotificationItems(notification, employee, objectId);

            foreach (var item in notificationItems)
            {
                await _writeRepository.Add(item);
            }
            await _unitOfWork.CompleteAsync(CancellationToken.None);
        }

        private IEnumerable<NotificationItem> GenerateNotificationItems(
            Notification notification,
            (string Id, string PrimaryApproverId, string SecondaryApproverId) employee,
            string objectId
            )
        {
            var populationsConcerned = _populationServices.Deconstruct(notification.Population);
            var employees = _populationServices
                .MatchPopulations((employee.Id, employee.PrimaryApproverId, employee.SecondaryApproverId));

            var employeeIds = populationsConcerned.Select(p => employees[p]).ToList();
            _populationServices.AddAdministratorsToEmployeesIfConcerned(populationsConcerned, employeeIds);

            var employeesConcerned = populationsConcerned.Select(p =>
                NotificationItem.Create(employees[p], notification.Action, ActionToSubject(notification.Action), false, objectId));

            return employeesConcerned;
        }

        public abstract string ActionToSubject(string action);
    }
}
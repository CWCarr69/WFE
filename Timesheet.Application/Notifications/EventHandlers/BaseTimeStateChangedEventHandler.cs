using Timesheet.Application.Notifications.Services;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Notifications.EventHandlers
{
    public abstract class BaseTimeStateChangedEventHandler
    {
        private readonly INotificationReadRepository _readRepository;
        private readonly IWriteRepository<NotificationItem> _writeRepository;

        private readonly INotificationPopulationServices _populationServices;


        protected BaseTimeStateChangedEventHandler(
            INotificationPopulationServices populationServices,
            INotificationReadRepository readRepository,
            IWriteRepository<NotificationItem> writeRepository)
        {
            this._readRepository = readRepository;
            this._writeRepository = writeRepository;
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
            var notificationItems = await GenerateNotificationItems(notification, employee, objectId);

            foreach (var item in notificationItems)
            {
                await _writeRepository.Add(item);
            }
            //await _unitOfWork.CompleteAsync(CancellationToken.None);
        }

        private async Task<IEnumerable<NotificationItem>> GenerateNotificationItems(
            Notification notification,
            (string Id, string PrimaryApproverId, string SecondaryApproverId) employee,
            string objectId
            )
        {
            var populationsConcerned = _populationServices.Deconstruct(notification.Population);
            var employees = _populationServices
                .MatchPopulations((employee.Id, employee.PrimaryApproverId, employee.SecondaryApproverId));

            var employeeIds = populationsConcerned
                .Where(p => employees.ContainsKey(p))
                .Select(p => employees[p])
                .Where(e => e is not null)
                .ToList();

            await _populationServices.AddAdministratorsToEmployeesIfConcerned(populationsConcerned, employeeIds);

            var notificationItems = employeeIds.Select(employeeId =>
                NotificationItem.Create(employee.Id, employeeId, notification.Action, ActionToSubject(notification.Action), false, objectId, notification.Group));

            return notificationItems;
        }

        public abstract string ActionToSubject(string action);
    }
}
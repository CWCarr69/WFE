using Timesheet.Application.Notifications.Services;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Employees;

namespace Timesheet.Application.Notifications.EventHandlers
{
    public class TimeoffStateChangedEventHandler : BaseTimeStateChangedEventHandler<TimeoffStateChanged>
    {
        public TimeoffStateChangedEventHandler(
            INotificationPopulationServices populationServices,
            INotificationReadRepository readRepository,
            IWriteRepository<NotificationItem> writeRepository,
            IUnitOfWork unitOfWork)
            : base(populationServices, readRepository, writeRepository, unitOfWork)
        {
        }

        public override async Task HandleEvent(TimeoffStateChanged @event)
        {
            await Handle(NotificationType.TIMEOFF,
                @event.EmployeeId,
                @event.PrimaryApproverId,
                @event.SecondaryApproverId,
                @event.Action,
                @event.ObjectId);
        }

        public override string ActionToSubject(string action)
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

            throw new InvalidOperationException($"Not an adequate action ({action}) for Timeoff state changed handler");
        }
    }
}

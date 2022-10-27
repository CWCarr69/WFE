using Timesheet.Application.Notifications.Services;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Notifications;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;
using Timesheet.DomainEvents.Timesheets;

namespace Timesheet.Application.Notifications.EventHandlers
{
    public class TimesheetStateChangedEventHandler : BaseTimeStateChangedEventHandler, IEventHandler<TimesheetStateChanged>
    {
        public TimesheetStateChangedEventHandler(
            INotificationPopulationServices populationServices,
            INotificationReadRepository readRepository,
            IWriteRepository<NotificationItem> writeRepository) 
            : base(populationServices, readRepository, writeRepository)
        {
        }

        public async Task Handle(TimesheetStateChanged @event)
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
            if (action == TimesheetEntryStatus.REJECTED.ToString())
            {
                return "Timesheet is rejected";
            }

            if (action == TimesheetEntryStatus.SUBMITTED.ToString())
            {
                return "Timesheet is submitted";
            }

            if (action == TimesheetEntryStatus.IN_PROGRESS.ToString())
            {
                return "Timesheet is created";
            }

            if (action == TimesheetEntryStatus.APPROVED.ToString())
            {
                return "Timesheet is approved";
            }

            throw new InvalidOperationException($"Not an adequate action ({action}) for Timesheet state changed handler");
        }
    }
}

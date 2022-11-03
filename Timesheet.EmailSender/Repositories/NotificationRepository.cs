using Timesheet.EmailSender.Models;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender.Repositories
{
    internal class NotificationRepository : INotificationRepository
    {
        private readonly IDatabaseService _dbServices;

        public NotificationRepository(IDatabaseService dbServices)
        {
            _dbServices = dbServices;
        }

        public void CompleteSend(IEnumerable<string> ids)
        {
            var sentParam = "@sent";
            _dbServices.Execute($"UPDATE notificationItems SET sent = {sentParam} Where id in ('{string.Join("','",ids)}')",
                new {sent = true});
        }

        public IEnumerable<TimeoffNotificationTemplate> GetTimeoffNotifications()
        {

            var sentParam = "@sent";
            var timeoffIdParam = "@timeoffHeaderId";

            string query = $@"
                SELECT TOP(100) DISTINCT
                n.Id AS {nameof(TimeoffNotificationTemplate.NotificationId)},
                t.Id AS {nameof(TimeoffNotificationTemplate.ItemId)},
                n.EmployeeId AS {nameof(TimeoffNotificationTemplate.EmployeeId)},
                n.Subject AS {nameof(TimeoffNotificationTemplate.Subject)},
                e.Fullname AS {nameof(TimeoffNotificationTemplate.EmployeeName)},
                m.Fullname AS {nameof(TimeoffNotificationTemplate.ManagerName)},
                t.CreatedDate AS {nameof(TimeoffNotificationTemplate.DateCreated)},
                t.EmployeeComment AS {nameof(TimeoffNotificationTemplate.EmployeeComment)},
                t.ApproverComment AS {nameof(TimeoffNotificationTemplate.SupervisorComment)},
                n.Action AS {nameof(TimeoffNotificationTemplate.Status)}
                FROM notificationItems n
                JOIN employees e on e.Id = n.EmployeeId
                JOIN employees m on e.PrimaryApproverId = m.Id
                JOIN timeoffHeader t on t.Id = n.ObjectId
                WHERE Sent = {sentParam}
            ";
            var timeoffs = _dbServices.Query<TimeoffNotificationTemplate>(query, new {sent = false});

            query = $@"
                SELECT 
                te.RequestDate AS {nameof(TimeoffEntryRowTemplate.Date)},
                te.Hours AS {nameof(TimeoffEntryRowTemplate.Hours)},
                te.Type AS {nameof(TimeoffEntryRowTemplate.VacationTypeId)},
                FROM notificationItems n
                JOIN employees e on e.Id = n.EmployeeId
                JOIN employees m on e.PrimaryApproverId = m.Id
                JOIN timeoffHeader t on t.Id = n.ObjectId
                JOIN timeoffEntry te on te.timeoffHeaderId = t.Id
                WHERE t.Id = {timeoffIdParam}
            ";

            foreach (var timeoff in timeoffs)
            {
                var entries = _dbServices.Query<TimeoffEntryRowTemplate>(query, new { timeoffId = timeoff.ItemId });
                timeoff.Rows = entries;
            }

            return timeoffs;
        }

        public IEnumerable<TimesheetNotificationTemplate> GetTimesheetNotifications()
        {
            var sentParam = "@sent";
            var timesheetIdParam = "@timeoffHeaderId";

            string query = $@"
                SELECT TOP(100) DISTINCT
                n.Id AS {nameof(TimeoffNotificationTemplate.NotificationId)},
                t.Id AS {nameof(TimesheetNotificationTemplate.ItemId)},
                n.EmployeeId AS {nameof(TimesheetNotificationTemplate.EmployeeId)},
                n.Subject AS {nameof(TimesheetNotificationTemplate.Subject)},
                e.Fullname AS {nameof(TimesheetNotificationTemplate.EmployeeName)},
                m.Fullname AS {nameof(TimesheetNotificationTemplate.ManagerName)},
                c.EmployeeComment AS {nameof(TimesheetNotificationTemplate.EmployeeComment)},
                c.ApproverComment AS {nameof(TimesheetNotificationTemplate.SupervisorComment)},
                t.PayrollPeriod AS {nameof(TimesheetNotificationTemplate.PayrollPeriod)},
                t.StartDate AS {nameof(TimesheetNotificationTemplate.PayrollStartDate)},
                t.EndDate AS {nameof(TimesheetNotificationTemplate.PayrollEndDate)},
                n.status AS {nameof(TimesheetNotificationTemplate.Status)}
                FROM notificationItems n
                JOIN employees e on e.Id = n.EmployeeId
                JOIN employees m on e.PrimaryApproverId = m.Id
                JOIN timesheets t on t.Id = n.ObjectId
                JOIN timesheetComment c on c.EmployeeId = e.Id and c.TimesheetId = t.Id
                WHERE Sent = {sentParam}
            ";
            var timesheets = _dbServices.Query<TimesheetNotificationTemplate>(query, new {sent = false});

            query = $@"
                SELECT 
                te.PayrollCode AS {nameof(TimesheetEntryRowTemplate.PayrollCode)},
                te.Hours AS {nameof(TimesheetEntryRowTemplate.Quantity)},
                te.ServiceOrderNumber AS {nameof(TimesheetEntryRowTemplate.ServiceOrderNo)},
                te.CustomerNumber AS {nameof(TimesheetEntryRowTemplate.CustomerName)},
                te.WorkDate AS {nameof(TimesheetEntryRowTemplate.WorkDate)},
                te.ProfitCenter AS {nameof(TimesheetEntryRowTemplate.ProfitCenter)},
                te.LaborCode AS {nameof(TimesheetEntryRowTemplate.LaborCode)},
                te.JobTaskNumber AS {nameof(TimesheetEntryRowTemplate.JobTaskNo)}
                FROM notificationItems n
                JOIN employees e on e.Id = n.EmployeeId
                JOIN timesheets t on t.Id = n.ObjectId
                JOIN timesheetEntry te on te.timesheetHeaderId = t.Id
                WHERE t.Id = {timesheetIdParam}
            ";

            foreach (var timesheet in timesheets)
            {
                var entries = _dbServices.Query<TimesheetEntryRowTemplate>(query, new { timesheetId = timesheet.ItemId });
                timesheet.TimesheetEntries = entries;
            }

            return timesheets;
        }
    }
}
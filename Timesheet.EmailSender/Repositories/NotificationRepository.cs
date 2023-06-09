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

        public void CompleteSend(params string[] ids)
        {
            var sentParam = "@sent";
            _dbServices.Execute($"UPDATE notificationItems SET sent = {sentParam} Where id in ('{string.Join("','",ids)}')",
                new {sent = true});
        }

        public IEnumerable<TimeoffNotificationTemplate> GetTimeoffNotifications(bool includeSent = false, bool takeFirst = false)
        {

            var sentParam = "@sent";
            var timeoffIdParam = "@timeoffHeaderId";

            string query = $@"
                SELECT DISTINCT TOP(100)
                n.Id AS {nameof(TimeoffNotificationTemplate.NotificationId)},
                t.Id AS {nameof(TimeoffNotificationTemplate.ItemId)},
                n.RelatedEmployeeId AS {nameof(TimeoffNotificationTemplate.RelatedEmployeeId)},
                n.EmployeeId AS {nameof(TimeoffNotificationTemplate.EmployeeId)},
                n.Subject AS {nameof(TimeoffNotificationTemplate.Subject)},
                e.Fullname AS {nameof(TimeoffNotificationTemplate.EmployeeName)},
                m.Fullname AS {nameof(TimeoffNotificationTemplate.ManagerName)},
                t.CreatedDate AS {nameof(TimeoffNotificationTemplate.DateCreated)},
                t.EmployeeComment AS {nameof(TimeoffNotificationTemplate.EmployeeComment)},
                t.ApproverComment AS {nameof(TimeoffNotificationTemplate.SupervisorComment)},
                n.Action AS {nameof(TimeoffNotificationTemplate.Status)}
                FROM notificationItems n
                JOIN employees e on e.Id = n.RelatedEmployeeId
                JOIN employees m on e.PrimaryApproverId = m.Id
                JOIN timeoffHeader t on t.Id = n.ObjectId
                {(includeSent ? "" : $"WHERE Sent = {sentParam}")}
            ";
            var timeoffs = _dbServices.Query<TimeoffNotificationTemplate>(query, new {sent = false});


            if (takeFirst && timeoffs.Any())
            {
                timeoffs = new List<TimeoffNotificationTemplate>() { timeoffs.FirstOrDefault() };
            }

            query = $@"
                SELECT DISTINCT te.Id,
                te.RequestDate AS {nameof(TimeoffEntryRowTemplate.Date)},
                te.Hours AS {nameof(TimeoffEntryRowTemplate.Hours)},
                pt.PayrollCode AS {nameof(TimeoffEntryRowTemplate.VacationType)}
                FROM timeoffEntry te
                JOIN payrollTypes pt on pt.NumId = te.TypeId
                WHERE te.timeoffHeaderId = {timeoffIdParam}
            ";

            foreach (var timeoff in timeoffs)
            {
                var entries = _dbServices.Query<TimeoffEntryRowTemplate>(query, new { timeoffHeaderId = timeoff.ItemId });
                timeoff.Rows = entries;
            }

            return timeoffs;
        }

        public IEnumerable<TimesheetNotificationTemplate> GetTimesheetNotifications(bool includeSent = false, bool takeFirst = false)
        {
            var sentParam = "@sent";
            var timesheetIdParam = "@timesheetId";

            string query = $@"
                SELECT DISTINCT TOP(100)
                n.Id AS {nameof(TimesheetNotificationTemplate.NotificationId)},
                t.Id AS {nameof(TimesheetNotificationTemplate.ItemId)},
                n.RelatedEmployeeId AS {nameof(TimesheetNotificationTemplate.RelatedEmployeeId)},
                n.EmployeeId AS {nameof(TimesheetNotificationTemplate.EmployeeId)},
                n.Subject AS {nameof(TimesheetNotificationTemplate.Subject)},
                e.Fullname AS {nameof(TimesheetNotificationTemplate.EmployeeName)},
                m.Fullname AS {nameof(TimesheetNotificationTemplate.ManagerName)},
                c.EmployeeComment AS {nameof(TimesheetNotificationTemplate.EmployeeComment)},
                c.ApproverComment AS {nameof(TimesheetNotificationTemplate.SupervisorComment)},
                t.PayrollPeriod AS {nameof(TimesheetNotificationTemplate.PayrollPeriod)},
                t.StartDate AS {nameof(TimesheetNotificationTemplate.PayrollStartDate)},
                t.EndDate AS {nameof(TimesheetNotificationTemplate.PayrollEndDate)},
                n.Action AS {nameof(TimesheetNotificationTemplate.Status)}
                FROM notificationItems n
                JOIN employees e on e.Id = n.RelatedEmployeeId
                JOIN employees m on e.PrimaryApproverId = m.Id
                JOIN timesheets t on t.Id = n.ObjectId
                LEFT JOIN timesheetComment c on c.EmployeeId = e.Id and c.TimesheetId = t.Id
                {(includeSent ? "" : $"WHERE Sent = {sentParam}")}
            ";
            var timesheets = _dbServices.Query<TimesheetNotificationTemplate>(query, new { sent = false });

            if (takeFirst && timesheets.Any())
            {
                timesheets = new List<TimesheetNotificationTemplate>() { timesheets.FirstOrDefault() };
            }

            query = $@"
                SELECT DISTINCt te.TimesheetEntryId,
                pt.PayrollCode AS {nameof(TimesheetEntryRowTemplate.PayrollCode)},
                te.TimesheetHeaderId AS {nameof(TimesheetEntryRowTemplate.TimesheetHeaderId)},
                te.Quantity AS {nameof(TimesheetEntryRowTemplate.Quantity)},
                te.ServiceOrderNumber AS {nameof(TimesheetEntryRowTemplate.ServiceOrderNo)},
                te.CustomerNumber AS {nameof(TimesheetEntryRowTemplate.CustomerName)},
                te.WorkDate AS {nameof(TimesheetEntryRowTemplate.WorkDate)},
                COALESCE(te.ProfitCenterNumber, e.DefaultProfitCenter) AS {nameof(TimesheetEntryRowTemplate.ProfitCenter)},
                te.LaborCode AS {nameof(TimesheetEntryRowTemplate.LaborCode)},
                te.JobTaskNumber AS {nameof(TimesheetEntryRowTemplate.JobTaskNo)},
                n.RelatedEmployeeId AS {nameof(TimesheetEntryRowTemplate.RelatedEmployeeId)}
                FROM notificationItems n
                JOIN employees e on e.Id = n.RelatedEmployeeId
                JOIN AllEmployeeTimesheetEntriesAndHolidays te on te.TimesheetHeaderId = n.ObjectId and te.EmployeeId = n.RelatedEmployeeId
                LEFT JOIN TimesheetException tex ON tex.TimesheetEntryId = te.TimesheetEntryId And tex.EmployeeId = n.RelatedEmployeeId
                JOIN payrollTypes pt on pt.NumId = te.PayrollCodeId
                WHERE tex.Id is null
            ";

            var entries = _dbServices.Query<TimesheetEntryRowTemplate>(query);
            var entriesPerTimesheetAndRelatedEmployee = entries.GroupBy(e => $"{e.RelatedEmployeeId}-{e.TimesheetHeaderId}")
                .ToDictionary(g => g.Key);

            if(timesheets != null)
            {
                timesheets.ForEach(timesheet =>
                {
                    if (entriesPerTimesheetAndRelatedEmployee.TryGetValue($"{timesheet.RelatedEmployeeId}-{timesheet.ItemId}", out var relatedEmployeeEntries))
                    {
                        timesheet.TimesheetEntries = relatedEmployeeEntries.ToList();
                    }
                });
            }

            return timesheets;
        }

        public TimesheetNotificationTemplate? GetTestTimesheetNotification()
        {
            return GetTimesheetNotifications(true, true).FirstOrDefault();
        }

        public TimeoffNotificationTemplate? GetTestTimeoffNotification()
        {
            return GetTimeoffNotifications(true, true).FirstOrDefault();
        }
    }
}
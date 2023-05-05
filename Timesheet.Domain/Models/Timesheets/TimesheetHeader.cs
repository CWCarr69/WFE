using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.DomainEvents.Timesheets;

namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetHeader : AggregateRoot
    {
        public TimesheetHeader(string id) : base(id)
        {
        }

        public TimesheetHeader(string id,
            string payrollPeriod,
            DateTime startDate,
            DateTime endDate,
            TimesheetType type,
            TimesheetStatus status) : base(id)
        {
            PayrollPeriod = payrollPeriod;
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
            Status = status;
        }

        public string PayrollPeriod { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public TimesheetType Type { get; private set; }
        public TimesheetStatus Status { get; private set; }



        public virtual ICollection<TimesheetEntry> TimesheetEntries { get; private set; } = new List<TimesheetEntry>();
        public IEnumerable<TimesheetEntry> TimesheetEntriesWithoutTimeoffs => TimesheetEntries?.Where(t => !t.IsTimeoff) ?? new List<TimesheetEntry>();
        public virtual ICollection<TimesheetHoliday> TimesheetHolidays { get; private set; } = new List<TimesheetHoliday>();
        public virtual ICollection<TimesheetComment> TimesheetComments { get; private set; } = new List<TimesheetComment>();
        public virtual ICollection<TimesheetException> TimesheetExceptions { get; private set; } = new List<TimesheetException>();

        public bool IsFinalizable => DateTime.Now >= EndDate;

        public static TimesheetHeader CreateMonthlyTimesheet(DateTime workDate)
        {
            var now = workDate;
            var isInSecondHalf = now.IsInSecondHalfOfMonth();
            var periodNumber = (now.Month - 1) * 2 + (isInSecondHalf ? 2 : 1);

            var payrollPeriod = BiWeeklyPayrollPeriod(now, periodNumber);
            var start = isInSecondHalf ? now.HalfDayOfMonth().AddDays(1) : now.FirstDayOfMonth();
            var end = isInSecondHalf ? now.LastDayOfMonth() : now.HalfDayOfMonth();

            return new TimesheetHeader(payrollPeriod, payrollPeriod, start, end, TimesheetType.SALARLY, TimesheetStatus.IN_PROGRESS);
        }

        public static TimesheetHeader CreateWeeklyTimesheet(DateTime workDate)
        {
            var now = workDate;
            var oneYearBefore = workDate.AddYears(-1);
            var oneYearAfter = workDate.AddYears(1);

            var fridayBeforeLastThursdayOfPreviousYear = oneYearBefore.LastDayOfYear(DayOfWeek.Thursday).Previous(DayOfWeek.Friday);
            var fridayBeforeLastThursdayOfCurrentYear = now.LastDayOfYear(DayOfWeek.Thursday).Previous(DayOfWeek.Friday);

            var firstPayrollStartDateOfYear = now >= fridayBeforeLastThursdayOfCurrentYear
                ? fridayBeforeLastThursdayOfCurrentYear
                : fridayBeforeLastThursdayOfPreviousYear;

            var nextThursdayFromNow = now.Next(DayOfWeek.Thursday);
            var numberOfWeeks = nextThursdayFromNow.WeeksBetweenDays(firstPayrollStartDateOfYear);

            var payrollPeriod = now >= fridayBeforeLastThursdayOfCurrentYear
                ? WeeklyPayrollPeriod(oneYearAfter.Year, numberOfWeeks)
                : WeeklyPayrollPeriod(now.Year, numberOfWeeks);

            var start = now.Previous(DayOfWeek.Friday);
            var end = nextThursdayFromNow;

            return new TimesheetHeader(payrollPeriod, payrollPeriod, start, end, TimesheetType.WEEKLY, TimesheetStatus.IN_PROGRESS);
        }

        public void AddTimesheetEntry(TimesheetEntry timesheetEntry)
        {
            if(timesheetEntry?.Id is null)
            {
                throw new ArgumentNullException(nameof(timesheetEntry));
            }
            this.TimesheetEntries.Add(timesheetEntry);
            this.UpdateMetadata();
        }

        public void Submit(Employee employee, string? comment)
        {
            ThrowNullArgumentExceptionIfNull(employee);

            UpdateComment(employee, timesheetComment => timesheetComment.UpdateEmployeeComment(comment));

            TransitionEntries(employee, entry => entry.Submit());
            this.UpdateMetadata();

            RaiseTimesheetWorkflowChangedEvent(employee, TimesheetEntryStatus.SUBMITTED.ToString());
        }

        public void Approve(Employee employee, string comment)
        {
            ThrowNullArgumentExceptionIfNull(employee);

            UpdateComment(employee, timesheetComment => timesheetComment.UpdateApproverComment(comment));

            TransitionEntries(employee, entry => entry.Approve());
            this.UpdateMetadata();

            RaiseTimesheetWorkflowChangedEvent(employee, TimesheetEntryStatus.APPROVED.ToString());
        }

        public void Reject(Employee employee, string comment)
        {
            ThrowNullArgumentExceptionIfNull(employee);

            UpdateComment(employee, timesheetComment => timesheetComment.UpdateApproverComment(comment));

            TransitionEntries(employee, entry => entry.Reject());
            this.UpdateMetadata();
            
            RaiseTimesheetWorkflowChangedEvent(employee, TimesheetEntryStatus.REJECTED.ToString());

            var timeoffEntries = TimesheetEntries.Where(t => t.IsTimeoff);
            if (timeoffEntries.Any())
            {
                RaiseDomainEvent(new TimesheetRejected
                (
                    EmployeeId: employee.Id,
                    Dates: timeoffEntries.Select(e => e.WorkDate).ToList())
                );
            }
        }

        public void FinalizeTimesheet()
        {
            if(!IsFinalizable)
            {
                throw new CannotFinalizeTImesheetException(PayrollPeriod);
            }

            //TimesheetEntries
            //    .Where(entry => entry.Status != TimesheetEntryStatus.REJECTED)
            //    .ToList()
            //    .ForEach(entry => entry.Status = TimesheetEntryStatus.APPROVED);

            this.Status = TimesheetStatus.FINALIZED;

            this.UpdateMetadata();
            
            RaiseDomainEvent(new TimesheetFinalized(this.PayrollPeriod));
        }

        public void AddHoliday(TimesheetHoliday timesheetHoliday)
        {
            if (timesheetHoliday?.Id is null)
            {
                throw new ArgumentNullException(nameof(timesheetHoliday));
            }
            var existingHoliday = this.TimesheetHolidays
                .FirstOrDefault(h => h.WorkDate == timesheetHoliday.WorkDate);

            if(existingHoliday is null)
            {
                this.TimesheetHolidays.Add(timesheetHoliday);
                this.UpdateMetadata();
            }
        }

        public void UpdateHoliday(string id, string description)
        {
            var holiday = this.TimesheetHolidays.FirstOrDefault(h => h.Id == Id);
            if(holiday is not null)
            {
                throw new EntityNotFoundException<TimesheetHoliday>(id);
            }
            holiday.Updated(description);
            this.UpdateMetadata();
        }

        public void DeleteHoliday(string id)
        {
            var holiday = this.TimesheetHolidays.FirstOrDefault(h => h.Id == Id);
            if (holiday is not null)
            {
                throw new EntityNotFoundException<TimesheetHoliday>(id);
            }
            TimesheetHolidays.Remove(holiday);
            this.UpdateMetadata();
        }

        public void DeleteTimesheet(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry.IsDeletable)
            {
                throw new TimesheetEntryIsNotDeletableException(timesheetEntry.Id);
            }
            this.TimesheetEntries.Remove(timesheetEntry);
            this.UpdateMetadata();
        }


        public void AddTimesheetException(string entryId, string employeeId, bool isHoliday)
        {
            var entry = TimesheetEntries.FirstOrDefault(t => t.Id == entryId);
            var holiday = TimesheetHolidays.FirstOrDefault(h => h.Id == entryId);

            if (!isHoliday && entry is null)
            {
                throw new EntityNotFoundException<TimesheetEntry>(nameof(TimesheetEntry));
            }

            if (isHoliday && holiday is null)
            {
                throw new EntityNotFoundException<TimesheetEntry>(nameof(TimesheetHoliday));
            }

            var timesheetException = new TimesheetException(entryId, employeeId, isHoliday ? nameof(TimesheetHoliday) : nameof(TimesheetEntry));

            this.TimesheetExceptions.Add(timesheetException);
            this.UpdateMetadata();

            RaiseDomainEvent(new TimesheetRejected(employeeId, new List<DateTime>() { entry.WorkDate }));
        }

        private void TransitionEntries(Employee employee, Action<TimesheetEntry> doTransition)
        {
            var entries = TimesheetEntries.Where(e => e.EmployeeId == employee?.Id);
            foreach (var entry in TimesheetEntries)
            {
                doTransition(entry);
            }
        }

        private void UpdateComment(Employee employee, Action<TimesheetComment> UpdateComment)
        {
            var timesheetComment = TimesheetComments
                .Where(c => c.EmployeeId == employee.Id).FirstOrDefault();
            if (timesheetComment is null)
            {
                timesheetComment = new TimesheetComment(employee.Id, Id);
            }
            UpdateComment(timesheetComment);
        }

        private static void ThrowNullArgumentExceptionIfNull(Employee employee)
        {
            if (employee is null || employee?.Id is null)
            {
                throw new ArgumentException(nameof(employee));
            }
        }

        private void RaiseTimesheetWorkflowChangedEvent(Employee employee, string status)
        {
            RaiseDomainEvent(new TimesheetStateChanged(employee.Id, employee.PrimaryApprover?.Id, employee.SecondaryApprover?.Id, status, this.Id));
        }

        private static string WeeklyPayrollPeriod(int year, int periodNumber) => $"{year}-{periodNumber}H";

        private static string BiWeeklyPayrollPeriod(DateTime date, int periodNumber) => $"{date.Year}-{periodNumber}SS";

        public void AddComment(Employee? employee, string comment)
        {
            UpdateComment(employee, timesheetComment => timesheetComment.UpdateEmployeeComment(comment));
        }

        public void AddApproverComment(Employee? employee, string comment)
        {
            UpdateComment(employee, timesheetComment => timesheetComment.UpdateApproverComment(comment));
        }
    }
}

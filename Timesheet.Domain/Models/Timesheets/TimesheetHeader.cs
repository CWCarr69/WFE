using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.DomainEvents.Timesheets;

namespace Timesheet.Domain.Models.Timesheets
{
    public class TimesheetHeader : AggregateRoot
    {
        public TimesheetHeader(string id,
            string payrollPeriod,
            DateTime startDate,
            DateTime endDate,
            TimesheetStatus status) : base(id)
        {
            PayrollPeriod = payrollPeriod;
            StartDate = startDate;
            EndDate = endDate;
            Status = status;
        }

        public string PayrollPeriod { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public TimesheetStatus Status { get; private set; }
        public virtual ICollection<TimesheetEntry> TimesheetEntries { get; private set; } = new List<TimesheetEntry>();

        public void AddTimesheetEntry(TimesheetEntry timesheetEntry)
        {
            if(timesheetEntry?.Id is null)
            {
                throw new ArgumentNullException(nameof(timesheetEntry));
            }
            this.TimesheetEntries.Add(timesheetEntry);
        }

        public void Submit(Employee employee)
        {
            TransitionEntries(employee, entry => entry.Submit());
            RaiseTimesheetWorkflowChangedEvent(employee, TimesheetEntryStatus.SUBMITTED.ToString());
        }

        public void Approve(Employee employee)
        {
            TransitionEntries(employee, entry => entry.Approve());
            RaiseTimesheetWorkflowChangedEvent(employee, TimesheetEntryStatus.APPROVED.ToString());
        }

        public void Reject(Employee employee)
        {
            TransitionEntries(employee, entry => entry.Reject());
            RaiseTimesheetWorkflowChangedEvent(employee, TimesheetEntryStatus.REJECTED.ToString());
        }

        public void Finalize()
        {
            if(EndDate >= DateTime.Now)
            {
                throw new CannotFinalizeTImesheetException(PayrollPeriod);
            }
            //TODO WHATEVER SHOULD BE DONE FOR ENTRIES
            this.Status = TimesheetStatus.FINALIZED;

            RaiseDomainEvent(new TimesheetFinalized(this.Id));
        }

        private void TransitionEntries(Employee employee, Action<TimesheetEntry> doTransition)
        {
            var entries = TimesheetEntries.Where(e => e.EmployeeId == employee?.Id);
            foreach (var entry in TimesheetEntries)
            {
                doTransition(entry);
            }
        }

        private void RaiseTimesheetWorkflowChangedEvent(Employee employee, string status)
        {
            RaiseDomainEvent(new TimesheetStateChanged(employee.Id, employee.PrimaryApprover?.Id, employee.SecondaryApprover?.Id, status, this.Id));
        }
    }
}

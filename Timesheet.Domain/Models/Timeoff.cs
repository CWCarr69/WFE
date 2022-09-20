using Timesheet.Exceptions;

namespace Timesheet.Domain.Models
{
    public class Timeoff : Entity
    {
        public Timeoff(string id, DateTime RequestStartDate,
            DateTime RequestEndDate,
            string EmployeeComment,
            string SupervisorComment
            ) : base(id)
        {
            this.RequestStartDate = RequestStartDate;
            this.RequestEndDate = RequestEndDate;
            this.EmployeeComment = EmployeeComment;
            this.SupervisorComment = SupervisorComment;
        }

        public DateTime RequestStartDate { get; private set; }
        public DateTime RequestEndDate { get; private set; }
        public string EmployeeComment { get; private set; }
        public string SupervisorComment { get; private set; }
        public TimeoffStatus Status { get; private set; }
        public TimeoffWorkflowHistory WorkflowHistory { get; private set; }
        public virtual ICollection<TimeoffEntry> TimeoffEntries { get; private set; } = new List<TimeoffEntry>();

        internal static Timeoff Create(DateTime requestStartDate, DateTime requestEndDate, string employeeComment, string supervisorComment)
        {
            var timeoff = new Timeoff(Guid.NewGuid().ToString(), requestStartDate, requestEndDate, employeeComment, supervisorComment);
            timeoff.Status = TimeoffStatus.IN_PROGRESS;

            return timeoff;
        }

        internal void Delete()
        {
            var removedEntries = TimeoffEntries;
            TimeoffEntries = new List<TimeoffEntry>();
        }

        internal void AddEntry(DateTime requestDate, TimeoffType type, double hours, Timeoff timeoff)
        {
            TimeoffEntries.Add(TimeoffEntry.Create(requestDate, type, hours));
        }

        internal void DeleteEntry(TimeoffEntry timeoffEntry)
        {
            TimeoffEntries.Remove(timeoffEntry);
        }

        internal void Approve(string comment) => DoTransition(TimeoffStatus.APPROVED, comment);

        internal void Submit(string comment) => DoTransition(TimeoffStatus.SUBMITTED, comment);

        internal void Reject(string comment) => DoTransition(TimeoffStatus.REJECTED, comment);

        internal void Update()
        {
            var timeoffStartDate = TimeoffEntries.Min(e => e.RequestDate);
            var timeoffEndDate = TimeoffEntries.Max(e => e.RequestDate);
            RequestStartDate = timeoffStartDate;
            RequestEndDate = timeoffEndDate;
        }

        private void DoTransition(TimeoffStatus status, string comment)
        {
            this.Status = status;
            this.EmployeeComment = comment ?? this.SupervisorComment;

            ValidateAllEntries();
        }

        private void ValidateAllEntries()
        {
            foreach (var entry in TimeoffEntries)
            {
                entry.Validate();
            }
        }

        public TimeoffEntry? GetTimeoffEntry(string timeoffEntryId) => TimeoffEntries.SingleOrDefault(e => e.Id == timeoffEntryId);

    }
}
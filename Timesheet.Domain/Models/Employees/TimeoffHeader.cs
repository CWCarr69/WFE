namespace Timesheet.Domain.Models.Employees
{
    public class TimeoffHeader : Entity
    {
        public TimeoffHeader(string id, DateTime requestStartDate,
            DateTime requestEndDate,
            string employeeComment
            ) : base(id)
        {
            this.RequestStartDate = requestStartDate;
            this.RequestEndDate = requestEndDate;
            this.EmployeeComment = employeeComment;
        }

        public DateTime RequestStartDate { get; private set; }

        public DateTime RequestEndDate { get; private set; }
        public string? EmployeeComment { get; private set; }
        public string? ApproverComment { get; private set; }
        public TimeoffStatus Status { get; private set; }
        public virtual ICollection<TimeoffEntry> TimeoffEntries { get; private set; } = new List<TimeoffEntry>();

        internal static TimeoffHeader Create(DateTime requestStartDate, DateTime requestEndDate, string employeeComment, bool requireApproval)
        {
            var timeoff = new TimeoffHeader(Guid.NewGuid().ToString(),
                requestStartDate.Date,
                requestEndDate.Date,
                employeeComment);

            timeoff.Status = requireApproval ? TimeoffStatus.SUBMITTED : TimeoffStatus.APPROVED;

            return timeoff;
        }

        internal void Delete()
        {
            var removedEntries = TimeoffEntries;
            TimeoffEntries = new List<TimeoffEntry>();
            this.UpdateMetadata();
        }

        internal TimeoffEntry AddEntry(DateTime requestDate, int TypeId, double hours, string label)
        {
            var entry = TimeoffEntry.Create(requestDate, TypeId, hours, label);
            TimeoffEntries.Add(entry);

            var timeoffAlreadyProcessed = this.Status == TimeoffStatus.REJECTED || Status == TimeoffStatus.APPROVED;
            if (timeoffAlreadyProcessed)
            {
                this.Status = TimeoffStatus.SUBMITTED;
            }
            this.UpdateMetadata();

            return entry;
        }

        internal void DeleteEntry(TimeoffEntry timeoffEntry)
        {
            TimeoffEntries.Remove(timeoffEntry);
            this.UpdateMetadata();
        }

        internal void Approve(string comment)
        {
            Transition(TimeoffStatus.APPROVED, () => this.ApproverComment = comment ?? this.ApproverComment);
            ValidateAllEntries();
            this.UpdateMetadata();
        }

        internal void Submit(string comment)
        {
            Transition(TimeoffStatus.SUBMITTED, () => this.EmployeeComment = comment ?? this.EmployeeComment);
            this.UpdateMetadata();
        }

        internal void Reject(string comment)
        {
            Transition(TimeoffStatus.REJECTED, () => this.ApproverComment = comment ?? this.ApproverComment);
            RejectAllEntries();
            this.UpdateMetadata();
        }

        internal void RejectEntries(DateTime date)
        {
            TimeoffEntries.Where(e => e.RequestDate == date)
                .ToList()
                .ForEach(e => e.Reject());

            if(TimeoffEntries.All(e => e.Status == TimeoffEntryStatus.REJECTED))
            {
                this.Reject("Timesheet Rejected");
            }

            this.UpdateMetadata();
        }

        internal void Update()
        {
            if (TimeoffEntries.Any())
            {
                var timeoffStartDate = TimeoffEntries.Min(e => e.RequestDate);
                var timeoffEndDate = TimeoffEntries.Max(e => e.RequestDate);
                RequestStartDate = timeoffStartDate;
                RequestEndDate = timeoffEndDate;
            }

            this.UpdateMetadata();
        }

        private void Transition(TimeoffStatus status, Action updateData)
        {
            this.Status = status;
            updateData();
        }

        private void ValidateAllEntries()
        {
            foreach (var entry in TimeoffEntries)
            {
                entry.Validate();
            }
        }

        private void RejectAllEntries()
        {
            foreach (var entry in TimeoffEntries)
            {
                entry.Reject();
            }
        }

        public TimeoffEntry? GetTimeoffEntry(string timeoffEntryId) => TimeoffEntries.SingleOrDefault(e => e.Id == timeoffEntryId);

        internal void AddComment(string comment)
        {
            this.EmployeeComment = comment;
            this.UpdateMetadata();
        }

        internal void AddApproverComment(string comment)
        {
            this.ApproverComment = comment;
            this.UpdateMetadata();
        }

    }
}
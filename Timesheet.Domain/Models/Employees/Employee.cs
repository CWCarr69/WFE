using Timesheet.Domain.Exceptions;
using Timesheet.DomainEvents.Employees;

namespace Timesheet.Domain.Models.Employees
{
    public class Employee : AggregateRoot
    {
        private const double EMPLOYEE_REGULAR_HOURS = 8;

        private List<TimeoffHeader> _timeoffs = new List<TimeoffHeader>();

        public Employee(string id) : base(id)
        {
        }

        public Employee(string id, string userId, string fullName,
            string managerId, string primaryApproverId, string secondaryApproverId,
            EmployeeEmploymentData employmentData, EmployeeContactData contacts, bool isActive) : base(id)
        {
            FullName = fullName;
            Manager = managerId is not null ? new Employee(managerId) : null;
            PrimaryApprover = primaryApproverId is not null ? new Employee(primaryApproverId) : null;
            SecondaryApprover = secondaryApproverId is not null ? new Employee(secondaryApproverId) : null;
            EmploymentData = employmentData;
            Contacts = contacts;
            IsActive = isActive;
            UserId = userId;
        }

        #region Properties
        public string FullName { get; private set; }
        public string? Initials { get; private set; }
        public Image? Picture { get; private set; }
        public Employee? Manager { get; private set; }
        public Employee? PrimaryApprover { get; private set; }
        public Employee? SecondaryApprover { get; private set; }
        public EmployeeEmploymentData EmploymentData { get; private set; }
        public EmployeeContactData Contacts { get; private set; }
        public EmployeeBenefits BenefitsVariation { get; private set; }
        public EmployeeBenefitsSnapshop BenefitsSnapshot { get; private set; }
        public bool ConsiderFixedBenefits { get; private set; }
        public IReadOnlyCollection<TimeoffHeader> Timeoffs => _timeoffs;
        public bool IsActive { get; private set; }
        public string UserId { get; private set; }
        #endregion

        public TimeoffStatus? LastTimeoffStatus()
        {
            if (!Timeoffs.Any())
            {
                return null;
            }

            var waitingValidations = Timeoffs.Any(t => t.Status == TimeoffStatus.SUBMITTED);

            if (waitingValidations)
            {
                return TimeoffStatus.SUBMITTED;
            }

            return Timeoffs.OrderBy(t => t.CreatedDate).Last()?.Status;
        }

        public void SetPrimaryApprover(Employee primaryApprover)
        {
            if (primaryApprover is not null)
            {
                this.PrimaryApprover = primaryApprover;
            }
        }

        public void SetSecondaryOfficer(Employee secondaryApprover)
        {
            if (secondaryApprover is not null)
            {
                this.SecondaryApprover = secondaryApprover;
            }
        }

        public void ChangeBenefitsCalculationMode(bool considerFixedBenefits)
        {
            ConsiderFixedBenefits = considerFixedBenefits;
        }

        public void SetBenefits(double vacationHours, double personalHours, double rolloverHours)
        {
            BenefitsVariation = new EmployeeBenefits(vacationHours, personalHours, rolloverHours);
        }

        public void SnapshotBenefits(EmployeeBenefitsSnapshop snapshot)
        {
            BenefitsSnapshot = snapshot;
        }

        #region Time Workflow
        public TimeoffHeader CreateTimeoff(DateTime requestStartDate, DateTime requestEndDate, string employeeComment, bool requireApproval)
        {
            var timeoff = TimeoffHeader.Create(requestStartDate, requestEndDate, employeeComment, requireApproval);
            _timeoffs.Add(timeoff);

            RaiseTimeoffWorkflowChangedEvent(timeoff, nameof(TimeoffStatus.IN_PROGRESS));
            RaiseTimeoffWorkflowChangedEvent(timeoff, nameof(TimeoffStatus.SUBMITTED));
            return timeoff;
        }

        public void DeleteTimeoff(TimeoffHeader timeoff)
        {
            timeoff.Delete();
            _timeoffs.Remove(timeoff);
        }

        public void SubmitTimeoff(TimeoffHeader timeoff, string comment)
        {
            timeoff.Submit(comment);
            RaiseTimeoffWorkflowChangedEvent(timeoff, nameof(TimeoffStatus.SUBMITTED));
        }

        public void ApproveTimeoff(TimeoffHeader timeoff, string comment)
        {
            timeoff.Approve(comment);
            RaiseTimeoffWorkflowChangedEvent(timeoff, nameof(TimeoffStatus.APPROVED));
            RaiseTimeoffApprovedEvent(timeoff);
        }

        public void RejectTimeoff(TimeoffHeader timeoff, string comment)
        {
            timeoff.Reject(comment);
            RaiseTimeoffWorkflowChangedEvent(timeoff, nameof(TimeoffStatus.REJECTED));
        }

        public void AddTimeoffEntry(DateTime requestDate, int typeId, double hours, TimeoffHeader timeoff, string label)
        {
            var timeoffEntriesOnSameDate = _timeoffs.SelectMany(t => t.TimeoffEntries)
                .Where(e => e.RequestDate.ToShortDateString() == requestDate.ToShortDateString())
                .ToList();

            var totalHoursOnSameDateExceedLimit = timeoffEntriesOnSameDate.Sum(e => e.Hours) + hours > EMPLOYEE_REGULAR_HOURS;

            if (totalHoursOnSameDateExceedLimit)
            {
                throw new TimeOffEntryHoursExceededException(requestDate, EMPLOYEE_REGULAR_HOURS);
            }

            var relatedTimeoff = _timeoffs.SingleOrDefault(t => t == timeoff);
            if (relatedTimeoff is null)
            {
                throw new EntityNotFoundException<TimeoffHeader>(timeoff.Id);
            }

            relatedTimeoff.AddEntry(requestDate, typeId, hours, label);
            relatedTimeoff.Update();
        }

        public void DeleteTimeoffEntry(TimeoffHeader timeoff, TimeoffEntry timeoffEntry)
        {
            timeoff.DeleteEntry(timeoffEntry);
            timeoff.Update();
        }

        public void UpdateTimeoffEntry(TimeoffHeader timeoff, TimeoffEntry timeoffEntry, int typeId, double hours, string label)
        {
            var timeoffEntriesOnSameDate = _timeoffs.SelectMany(t => t.TimeoffEntries)
                .Where(e => e != timeoffEntry && e.RequestDate.ToShortDateString() == timeoffEntry.RequestDate.ToShortDateString())
                .ToList();

            var totalHoursOnSameDateExceedLimit = timeoffEntriesOnSameDate.Sum(e => e.Hours) + hours > EMPLOYEE_REGULAR_HOURS;

            if (totalHoursOnSameDateExceedLimit)
            {
                throw new TimeOffEntryHoursExceededException(timeoffEntry.RequestDate, EMPLOYEE_REGULAR_HOURS);
            }

            timeoffEntry.Update(typeId, hours, label);

            timeoff.Update();
        }

        public void AddComment(TimeoffHeader timeoff, string employeeComment)
        {
            timeoff.AddComment(employeeComment);
            this.UpdateMetadata();
        }

        public void AddApproverComment(TimeoffHeader timeoff, string approverComment)
        {
            timeoff.AddApproverComment(approverComment);
        }
        #endregion

        //public bool IsInEmployeeTeam(string teamApproverId, bool directReports = false)
        //{
        //    if (directReports)
        //    {
        //        string? directApproverId = Supervisor?.Id ?? Manager?.Id;
        //        return teamApproverId != null && teamApproverId == directApproverId;
        //    }

        //    return teamApproverId != null
        //    && (Manager?.Id == teamApproverId || Supervisor?.Id == teamApproverId);
        //}

        private void RaiseTimeoffWorkflowChangedEvent(TimeoffHeader timeoff, string status)
        {
            RaiseDomainEvent(new TimeoffStateChanged(Id, PrimaryApprover?.Id, SecondaryApprover?.Id, status, timeoff.Id)); ;
        }

        private void RaiseTimeoffApprovedEvent(TimeoffHeader timeoff)
        {
            List<TimeoffApprovedEntry> timeoffEntries = new();

            foreach (var entry in timeoff.TimeoffEntries)
            {
                timeoffEntries.Add(new TimeoffApprovedEntry(
                    entry.Id,
                    Id,
                    entry.RequestDate,
                    entry.TypeId,
                    entry.Hours,
                    entry.TypeId.ToString(),
                    this.EmploymentData.IsSalaried));
            }

            RaiseDomainEvent(new TimeoffApproved(timeoffEntries));
        }

        public TimeoffHeader? GetTimeoff(string timeoffId) => _timeoffs.SingleOrDefault(t => t.Id == timeoffId);

        public TimeoffEntry? GetTimeoffEntry(string timeoffId, string timeoffEntryId)
        {
            var timeoff = GetTimeoff(timeoffId);
            return timeoff?.GetTimeoffEntry(timeoffEntryId);
        }
    }
}



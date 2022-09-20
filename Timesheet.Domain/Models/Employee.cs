using Timesheet.Domain.Exceptions;

namespace Timesheet.Domain.Models
{
    public class Employee : AggregateRoot
    {
        private const double EMPLOYEE_REGULAR_HOURS = 8;

        private IList<Timeoff> _timeoffs = new List<Timeoff>();

        public Employee(string id) : base(id)
        {
        }

        public string Firstname { get; private set; }
        public string Middlename { get; private set; }
        public string Lastname { get; private set; }
        public string FullName { get; private set; }
        public string Initials { get; private set; }
        public Image Picture { get; private set; }
        public EmployeeEmploymentData EmploymentData { get; private set; }
        public EmployeePersonalData PersonalData { get; private set; }
        public EmployeeBenefits Benefits { get; private set; }
        public IReadOnlyCollection<Timeoff> Timeoffs { get; set; }
        public EmployeeStatus Status { get; private set; }
        public TimeoffStatus LastTimeoffStatus { get; private set; }
        public TimesheetStatus LastTimesheetStatus { get; private set; }

        public string UserId { get; private set; }

        #region Workflow
        public void CreateTimeoff(DateTime requestStartDate, DateTime requestEndDate, string employeeComment, string supervisorComment)
        {
            var timeoff = Timeoff.Create(requestStartDate, requestEndDate, employeeComment, supervisorComment);
            _timeoffs.Add(timeoff);
        }

        public void DeleteTimeoff(Timeoff timeoff)
        {
            timeoff.Delete();
            _timeoffs.Remove(timeoff);
        }

        public void SubmitTimeoff(Timeoff timeoff, string comment) => timeoff.Submit(comment);

        public void ApproveTimeoff(Timeoff timeoff, string comment) => timeoff.Approve(comment);

        public void RejectTimeoff(Timeoff timeoff, string comment) => timeoff.Reject(comment);

        public void AddTimeoffEntry(DateTime requestDate, TimeoffType type, double hours, string timeoffId)
        {
            var timeoffEntriesOnSameDate = _timeoffs.SelectMany(t => t.TimeoffEntries)
                .Where(e => e.RequestDate.ToShortDateString() == requestDate.ToShortDateString())
                .ToList();

            var totalHoursOnSameDateExceedLimit = timeoffEntriesOnSameDate.Sum(e => e.Hours) + hours > EMPLOYEE_REGULAR_HOURS;

            if (totalHoursOnSameDateExceedLimit)
            {
                throw new TimeOffEntryHoursExceededException(requestDate, EMPLOYEE_REGULAR_HOURS);
            }

            var timeoff = _timeoffs.SingleOrDefault(t => t.Id == timeoffId);
            if (timeoff is null)
            {
                throw new EntityNotFoundException<Timeoff>(timeoffId);
            }

            timeoff.AddEntry(requestDate, type, hours, timeoff);
        }

        public void DeleteTimeoffEntry(Timeoff timeoff, TimeoffEntry timeoffEntry) => timeoff.DeleteEntry(timeoffEntry);

        public void UpdateTimeoffEntry(Timeoff timeoff, TimeoffEntry timeoffEntry, TimeoffType type, double hours)
        {
            var timeoffEntriesOnSameDate = _timeoffs.SelectMany(t => t.TimeoffEntries)
                .Where(e => e != timeoffEntry && e.RequestDate.ToShortDateString() == timeoffEntry.RequestDate.ToShortDateString())
                .ToList();

            var totalHoursOnSameDateExceedLimit = timeoffEntriesOnSameDate.Sum(e => e.Hours) + hours > EMPLOYEE_REGULAR_HOURS;

            if (totalHoursOnSameDateExceedLimit)
            {
                throw new TimeOffEntryHoursExceededException(timeoffEntry.RequestDate, EMPLOYEE_REGULAR_HOURS);
            }

            timeoffEntry.Update(type, hours);

            timeoff.Update();
        }
        #endregion

        public Timeoff? GetTimeoff(string timeoffId) => _timeoffs.SingleOrDefault(t => t.Id == timeoffId);

        public TimeoffEntry? GetTimeoffEntry(string timeoffId, string timeoffEntryId)
        {
            var timeoff = GetTimeoff(timeoffId);
            return timeoff?.GetTimeoffEntry(timeoffEntryId);
        }
    }
}



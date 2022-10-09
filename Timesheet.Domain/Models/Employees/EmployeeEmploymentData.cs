namespace Timesheet.Domain.Models.Employees
{
    public class EmployeeEmploymentData: ValueObject
    {
        public EmployeeEmploymentData(string jobTitle, string department, DateTime employmentDate, DateTime? terminationDate, bool isSalaried, bool isAdministrator)
        {
            JobTitle = jobTitle;
            Department = department;
            EmploymentDate = employmentDate;
            TerminationDate = terminationDate;
            IsSalaried = isSalaried;
            IsAdministrator = isAdministrator;
        }
        public string JobTitle { get; private set; }
        public string Department { get; private set; }
        public DateTime EmploymentDate { get; private set; }
        public DateTime? TerminationDate { get; private set; }
        public bool IsSalaried { get; private set; }
        public bool IsAdministrator { get; private set; }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return JobTitle;
            yield return Department;
            yield return EmploymentDate;
            yield return TerminationDate;
            yield return IsSalaried;
            yield return IsAdministrator;
        }
    }
}

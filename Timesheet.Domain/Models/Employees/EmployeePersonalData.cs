namespace Timesheet.Domain.Models.Employees
{
    public class EmployeePersonalData : ValueObject
    {
        public string CompanyEmail { get; set; }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return CompanyEmail;
        }
    }
}
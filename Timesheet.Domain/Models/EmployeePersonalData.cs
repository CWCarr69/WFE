namespace Timesheet.Domain.Models
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
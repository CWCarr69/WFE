namespace Timesheet.Domain.Models.Employees
{
    public class EmployeeContactData : ValueObject
    {
        public EmployeeContactData(string companyEmail, string companyPhone)
        {
            CompanyEmail = companyEmail;
            CompanyPhone = companyPhone;
        }

        public string CompanyEmail { get; private set; }
        public string CompanyPhone { get; private set; }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return CompanyEmail;
            yield return CompanyPhone;
        }
    }
}
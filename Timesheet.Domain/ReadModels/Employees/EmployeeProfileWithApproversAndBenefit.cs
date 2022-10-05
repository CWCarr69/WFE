namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeProfile
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public DateTime? HireDate { get; set; }
        public byte[]? Picture { get; set; }
    }
}

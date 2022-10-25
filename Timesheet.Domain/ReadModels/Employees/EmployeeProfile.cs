namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeProfile
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public byte[]? Picture { get; set; }
        public bool IsAdministrator { get; set; }
    }
}

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeeProfile
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public int CumulatedPreviousWorkPeriod { get; set; }
        public byte[]? Picture { get; set; }
        public bool IsAdministrator { get; set; }
        public bool ConsiderFixedBenefits { get; set; }
        public string? PrimaryApproverId { get; set; }
        public string? SecondaryApproverId { get; set; }
        public bool IsSalaried { get; set; }
        public bool IsManager { get; set; }

    }
}

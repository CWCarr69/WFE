
using Timesheet.Domain.Models;

namespace Timesheet.ReadModel.ReadModels
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

    public class Approver
    {
        public string Id { get; set; }
        public string FullName { get; set; }
    }

    public class HourInformation
    {
        public int Balance { get; set; }
        public int Used { get; set; }
        public int Scheduled { get; set; } 
    }

    public class HoursInformations
    {
        public int EligibleVacationHours { get; set; }
        public int EligiblePersonalHours { get; set; }
        public int RolloverHours { get; set; }

        public HourInformation VacationHours { get; set; }
        public HourInformation PersonalHours { get; set; }
    }

    public class EmployeeProfileWithApproversAndBenefit
    {
        public EmployeeProfileWithApproversAndBenefit(Employee employee)
        {
            var profile = new EmployeeProfile
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Login = string.Empty,
                Department = employee.EmploymentData?.Department,
                HireDate = employee.EmploymentData?.EmploymentDate,
                Picture = employee.Picture?.Data
            };

            var primaryApprover = new Approver { FullName = employee.EmploymentData?.Supervisor.FullName };
            var secondaryApprover = new Approver { FullName = employee.EmploymentData?.Manager.FullName };

            var hours = new HoursInformations();

            Profile = profile;
            PrimaryApprover = primaryApprover;
            SecondaryApprover = secondaryApprover;
            Hours = hours;
        }

        public EmployeeProfile Profile { get; set; }
        public Approver PrimaryApprover { get; set; }
        public Approver SecondaryApprover { get; set; }
        public HoursInformations Hours { get; set; }

        public static explicit operator EmployeeProfileWithApproversAndBenefit(Employee e)
            => new EmployeeProfileWithApproversAndBenefit(e);
    }
}

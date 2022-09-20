using Timesheet.ReadModel.ReadModels;

namespace Timesheet.ReadModel.Queries
{
    public interface IQueryEmployee
    {
        EmployeeProfileWithApproversAndBenefit? GetEmployeeProfile(string id);
        IEnumerable<EmployeeWithTimeStatus> GetEmployeesWithTimeRecordStatus(string? approverId = null, bool directReports = false);
        public IEnumerable<EmployeeWithPendingTimeoffs> GetEmployeesWithPendingTimeoffs(string? approverId = null, bool directReports = false);
    }
}

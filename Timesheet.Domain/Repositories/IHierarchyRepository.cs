namespace Timesheet.Domain.Repositories
{
    public interface IHierarchyRepository
    {
        Task<bool> IsEmployeeManager(string employeeId, string managerId);
    }
}

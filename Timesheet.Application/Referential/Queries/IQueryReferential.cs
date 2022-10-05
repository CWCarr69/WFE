
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Referential.Queries
{
    public interface IQueryReferential
    {
        Task<IEnumerable<EmployeeLight>> GetEmployees();
        Task<IEnumerable<TimeoffType>> GetTimeoffTypes();
        Task<IEnumerable<PayrollPeriod>> GetPayrollPeriods();
        Task<IEnumerable<Department>> GetDepartments();
        Task<IEnumerable<TimesheetStatus>> GetTimesheetStatuses();
    }
}

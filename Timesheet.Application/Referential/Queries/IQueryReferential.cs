
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Referential.Queries
{
    public interface IQueryReferential
    {
        Task<IEnumerable<PayrollPeriod>> GetPayrollPeriods();
        Task<IEnumerable<Department>> GetDepartments();
        IEnumerable<EnumReadModel<TimesheetStatus>> GetTimesheetStatuses();
        IEnumerable<EnumReadModel<TimesheetEntryStatus>> GetTimesheetEntryStatuses();
        IEnumerable<EnumReadModel<TimeoffStatus>> GetTimeoffStatuses();
        IEnumerable<EnumReadModel<TimeoffType>> GetTimeoffTypes();
        Task<IEnumerable<string>> GetTimeoffLabels();
    }
}

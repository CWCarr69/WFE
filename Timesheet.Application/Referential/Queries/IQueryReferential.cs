
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Referential;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Models.Referential;
using PayrollType = Timesheet.Domain.ReadModels.Referential.PayrollType;

namespace Timesheet.Application.Referential.Queries
{
    public interface IQueryReferential
    {
        Task<IEnumerable<PayrollPeriod>> GetPayrollPeriods();
        Task<IEnumerable<Department>> GetDepartments();
        IEnumerable<EnumReadModel<TimesheetStatus>> GetTimesheetStatuses();
        IEnumerable<EnumReadModel<TimesheetEntryStatus>> GetTimesheetEntryStatuses();
        IEnumerable<EnumReadModel<TimeoffStatus>> GetTimeoffStatuses();
        Task<IEnumerable<PayrollType>> GetTimeoffTypes(bool requireApproval = true);
        Task<IEnumerable<string>> GetTimeoffLabels();
        Task<IEnumerable<PayrollType>> GetPayrollCodes();
        Task<IEnumerable<PayrollType>> GetAllTimeoffTypes();
        Task<IEnumerable<SimpleDictionaryItem>> GetJobs();
        Task<IEnumerable<SimpleDictionaryItem>> GetJobTasks();
        Task<IEnumerable<SimpleDictionaryItem>> GetServiceOrders();
        Task<IEnumerable<string>> GetCustomerNumbers();
        Task<IEnumerable<string>> GetProfitCenters();
        Task<IEnumerable<string>> GetLaborCodes();
    }
}

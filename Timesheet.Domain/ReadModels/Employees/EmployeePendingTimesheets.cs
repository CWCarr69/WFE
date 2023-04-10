using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Domain.ReadModels.Employees
{
    public class EmployeePendingTimesheets : WithTotal<EmployeeTimesheet>
    {
    }

    public class EmployeeOrphanTimesheets : WithTotal<EmployeeTimesheetWhithHoursPerStatus>
    {
    }
}

using Microsoft.EntityFrameworkCore;
using Timesheet.ReadModel.Queries;
using Timesheet.ReadModel.ReadModels;

namespace Timesheet.Infrastructure.Persistence.Queries
{
    internal class QueryEmployee : IQueryEmployee
    {
        private readonly TimesheetDbContext _context;

        public QueryEmployee(TimesheetDbContext context)
        {
            this._context = context;
        }

        public EmployeeProfileWithApproversAndBenefit? GetEmployeeProfile(string id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return null;
            }

            return (EmployeeProfileWithApproversAndBenefit) employee;
        }

        public IEnumerable<EmployeeWithTimeStatus> GetEmployeesWithTimeRecordStatus(string approverId = null, bool directReports = false)
        {
            var onlyDirectReports = approverId != null && directReports;

            var employees = _context.Employees
                .Where(e => approverId == null && e.IsInEmployeeTeam(approverId, onlyDirectReports))
                .Include(e => e.Timeoffs.Where(t => t.Status != Domain.Models.TimeoffStatus.IN_PROGRESS));

            var employeesWithTimeStatus = employees.Select(e => (EmployeeWithTimeStatus)e);
            return employeesWithTimeStatus;
        }

        public IEnumerable<EmployeeWithPendingTimeoffs> GetEmployeesWithPendingTimeoffs(string approverId = null, bool directReports = false)
        {
            var onlyDirectReports = approverId != null && directReports;

            var employees = _context.Employees
                .Where(e => approverId == null && e.IsInEmployeeTeam(approverId, onlyDirectReports))
                .Include(e => e.Timeoffs.Where(timeoff => timeoff.Status == Domain.Models.TimeoffStatus.APPROVED))
                .Where(e => e.Timeoffs.Any());

            var employeesWithPendingTimeoffs = employees.Select(e => new EmployeeWithPendingTimeoffs(e));

            return employeesWithPendingTimeoffs;
        }
    }
}

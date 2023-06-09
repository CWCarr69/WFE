using Timesheet.EmailSender.Models;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.EmailSender.Repositories
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDatabaseService _dbServices;

        public EmployeeRepository(IDatabaseService dbServices)
        {
            _dbServices = dbServices;
        }

        public IDictionary<string, string> GetEmails()
        {
            var query = "SELECT id, companyEmail FROM employees WHERE userId LIKE '%@%.%'";
            var emails = _dbServices.Query<(string Id, string Email)>(query).ToDictionary(r => r.Id, r=> r.Email);
            return emails;
        }
    }
}
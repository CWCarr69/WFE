﻿using Timesheet.Domain.Models.Employees;

namespace Timesheet.Domain.Repositories
{
    public interface IEmployeeReadRepository : IReadRepository<Employee>
    {
        Task<Employee?> GetEmployee(string id);
        Task<IEnumerable<Employee>> GetEmployeeWithTimeoffsInPeriod(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Employee>> GetAdministrators();
        Task<IDictionary<string, string>> GetEmployeesDictionary();
    }
}

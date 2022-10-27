using Microsoft.Extensions.Configuration;
using Timesheet.Domain.Models.Employees;
using Timesheet.FDPDataIntegrator.Services;

namespace Timesheet.FDPDataIntegrator.Employees
{
    internal class EmployeeAdapter : IAdapter<EmployeeRecord, Employee> 
    {
        private readonly List<String> _administrators;
        public EmployeeAdapter(IConfiguration configuration)
        {
            _administrators = configuration.GetSection("Administrators")
                .AsEnumerable()
                .Where(a => a.Value is not null)
                .Select(a => a.Value)
                .ToList();
        }

        public Employee Adapt(EmployeeRecord record)
        {
            if(record == null || string.IsNullOrEmpty(record.EmployeeCode))
            {
                throw new ArgumentNullException(nameof(record));
            }

            var employmentData = new EmployeeEmploymentData(
                jobTitle: record.Title,
                department: record.Department,
                employmentDate: record.CreateDate,
                terminationDate: null,
                isSalaried: record.TimesheetPeriod != "WEEKLY",
                isAdministrator: /*record.JobRole == "ADMIN"*/ _administrators is not null && _administrators
                    .Any(a => record.ADLogin is not null 
                        && record.ADLogin.ToLower().StartsWith(a.ToLower()))
            );

            var employeeContactData = new EmployeeContactData(companyEmail: record.Email, companyPhone:record.Phone);

            var employee = new Employee(
                record.EmployeeCode,
                record.ADLogin,
                record.FullName,
                record.ManagerId,
                record.ManagerId,
                record.SecondaryApproverId,
                employmentData,
                employeeContactData,
                record.Active != "False"
            );
			
			employee.UpdateMetadata(record.CreateDate, record.ModifyDate);

            return employee;
        }
    }
}

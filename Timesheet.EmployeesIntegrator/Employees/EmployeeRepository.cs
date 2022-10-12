using Timesheet.Domain.Models.Employees;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Employees
{
    internal class EmployeeRepository : IRepository<Employee>
    {
        private const string EmployeeTableName = "Employees";
        private readonly IDatabaseService _databaseService;

        public EmployeeRepository(IDatabaseService databaseService)
        {
            this._databaseService = databaseService;
        }

        public async Task BeginTransaction(Action transaction)
        {
            await _databaseService.ExecuteTransactionAsync(transaction);
        }

        public async Task DisableConstraints()
        {
            var employeeTableParam = "@tableName";
            var query = $"ALTER TABLE Employees NOCHECK CONSTRAINT ALL";
            await _databaseService.ExecuteAsync(query, new { tableName = EmployeeTableName });
        }

        public async Task EnableConstraints()
        {
            var employeeTableParam = "@tableName";
            var query = $"ALTER TABLE {employeeTableParam} WITH CHECK CHECK CONSTRAINT ALL";
            await _databaseService.ExecuteAsync(query, new { tableName = EmployeeTableName });
        }

        public async Task UpSert(Employee employee)
        {
            var employeeTable = "@tableName";
            var employeeId = "@employeeId";
            var employeeFullName = "@employeeFullName";
            var employeeManagerId = "@employeeManagerId";
            var employeePrimaryApproverId = "@employeePrimaryApproverId";
            var employeeSecondaryApproverId = "@employeeSecondaryApproverId";
            var employeeJobTitle = "@employeeJobTitle";
            var employeeDepartment = "@employeeDepartment";
            var employeeEmploymentDate = "@employeeEmploymentDate";
            var employeeIsSalaried = "@employeeIsSalaried";
            var employeeIsAdministrator = "@employeeIsAdministrator";
            var employeeCompanyEmail = "@employeeCompanyEmail";
            var employeeCompanyPhone = "@employeeCompanyPhone";
            var employeeCreatedDate = "@employeeCreatedDate";
            var employeeModifiedDate = "@employeeModifiedDate";

            var updates = $@"
            {nameof(Employee.FullName)} = {employeeFullName},
            {nameof(Employee.Manager)}{nameof(Employee.Id)} = {employeeManagerId},
            {nameof(Employee.PrimaryApprover)}{nameof(Employee.Id)} = {employeePrimaryApproverId},
            {nameof(Employee.SecondaryApprover)}{nameof(Employee.Id)} = {employeeSecondaryApproverId},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.JobTitle)} = {employeeJobTitle},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.Department)} = {employeeDepartment},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.EmploymentDate)} = {employeeEmploymentDate},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.IsSalaried)} = {employeeIsSalaried},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.IsAdministrator)} = {employeeIsAdministrator},
            {nameof(Employee.Contacts)}_{nameof(EmployeeContactData.CompanyEmail)} = {employeeCompanyEmail},
            {nameof(Employee.Contacts)}_{nameof(EmployeeContactData.CompanyPhone)} = {employeeCompanyPhone},
            {nameof(Employee.ModifiedDate)} = {employeeModifiedDate}
            ";

            var insertColums = $@"
            {nameof(Employee.Id)},
            {nameof(Employee.FullName)},
            {nameof(Employee.Manager)}{nameof(Employee.Id)},
            {nameof(Employee.PrimaryApprover)}{nameof(Employee.Id)},
            {nameof(Employee.SecondaryApprover)}{nameof(Employee.Id)},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.JobTitle)},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.Department)},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.EmploymentDate)},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.IsSalaried)},
            {nameof(Employee.EmploymentData)}_{nameof(EmployeeEmploymentData.IsAdministrator)},
            {nameof(Employee.Contacts)}_{nameof(EmployeeContactData.CompanyEmail)},
            {nameof(Employee.Contacts)}_{nameof(EmployeeContactData.CompanyPhone)},
            {nameof(Employee.CreatedDate)},
            {nameof(Employee.ModifiedDate)}
            ";

            var insertValues = $@"
                {employeeId},
                {employeeFullName},
                {employeeManagerId},
                {employeePrimaryApproverId},
                {employeeSecondaryApproverId},
                {employeeJobTitle},
                {employeeDepartment},
                {employeeEmploymentDate},
                {employeeIsSalaried},
                {employeeIsAdministrator},
                {employeeCompanyEmail},
                {employeeCompanyPhone},
                {employeeCreatedDate},
                {employeeModifiedDate}
            ";

            var query = $@"IF EXISTS (SELECT * FROM {employeeTable} WHERE {nameof(Employee.Id)} = {employeeId})
                         BEGIN
                             UPDATE {employeeTable}
                             SET {updates}
                             WHERE {nameof(Employee.Id)} = {employeeId};
                                    END
                                    ELSE
                         BEGIN
                             INSERT INTO {employeeTable} ({insertColums})
                             SELECT {insertValues}
                         END";

            await _databaseService.ExecuteAsync(query, new { 
                tableName = EmployeeTableName,
                employeeId = employee.Id,
                employeeFullName = employee.FullName,
                employeeManagerId = employee.PrimaryApprover?.Id,
                employeePrimaryApproverId = employee.PrimaryApprover?.Id,
                employeeSecondaryApproverId = employee.SecondaryApprover?.Id,
                employeeJobTitle = employee.EmploymentData.JobTitle,
                employeeDepartment = employee.EmploymentData.Department,
                employeeEmploymentDate = employee.EmploymentData.EmploymentDate,
                employeeIsSalaried = employee.EmploymentData.IsSalaried,
                employeeIsAdministrator = employee.EmploymentData.IsAdministrator,
                employeeCompanyEmail = employee.Contacts.CompanyEmail,
                employeeCompanyPhone = employee.Contacts.CompanyPhone,
            });
        }
    }
}

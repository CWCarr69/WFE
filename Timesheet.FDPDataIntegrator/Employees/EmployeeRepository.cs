﻿using Timesheet.Domain.Models.Employees;
using Timesheet.FDPDataIntegrator.Services;
using Timesheet.Infrastructure.Dapper;

namespace Timesheet.FDPDataIntegrator.Employees
{
    internal class EmployeeRepository : IRepository<Employee>
    {
        private const string EmployeeTable = "Employees";
        private readonly IDatabaseService _databaseService;

        public EmployeeRepository(IDatabaseService databaseService)
        {
            this._databaseService = databaseService;
        }

        public async Task BeginTransaction(Action transaction)
        {
            await _databaseService.ExecuteTransactionAsync(transaction);
        }

        public async Task Delete(string id)
        {
            await Task.CompletedTask;
        }

        public async Task DisableConstraints()
        {
            var employeeTableParam = "@tableName";
            var query = $"ALTER TABLE {EmployeeTable} NOCHECK CONSTRAINT ALL";
            await _databaseService.ExecuteAsync(query, new { tableName = EmployeeTable });
        }

        public async Task EnableConstraints()
        {
            var employeeTableParam = "@tableName";
            var query = $"ALTER TABLE {EmployeeTable} WITH CHECK CHECK CONSTRAINT ALL";
            await _databaseService.ExecuteAsync(query, new { tableName = EmployeeTable });
        }

        public IEnumerable<Employee> GetRecents()
        {
            throw new NotImplementedException();
        }

        public async Task UpSert(Employee employee)
        {
            var employeeId = "@employeeId";
            var employeeFullName = "@employeeFullName";
            var employeeDefaultProfitCenter = "@employeeDefaultProfitCenter";
            var employeeManagerId = "@employeeManagerId";
            var employeePrimaryApproverId = "@employeePrimaryApproverId";
            var employeeSecondaryApproverId = "@employeeSecondaryApproverId";
            var employeeJobTitle = "@employeeJobTitle";
            var employeeDepartment = "@employeeDepartment";
            var employeeEmploymentDate = "@employeeEmploymentDate";
            var employeeIsSalaried = "@employeeIsSalaried";
            var employeeIsAdministrator = "@employeeIsAdministrator";
            var employeeIsActive = "@employeeIsActive";
            var employeeUsesTimesheet = "@employeeUsesTimesheet";
            var employeeCompanyEmail = "@employeeCompanyEmail";
            var employeeCompanyPhone = "@employeeCompanyPhone";
            var employeeCreatedDate = "@employeeCreatedDate";
            var employeeModifiedDate = "@employeeModifiedDate";
            var employeeUpdatedBy = "@employeeUpdatedBy";
            var employeeUserId = "@employeeUserId";

            var updates = $@"
            {nameof(Employee.FullName)} = {employeeFullName},
            {nameof(Employee.DefaultProfitCenter)} = {employeeDefaultProfitCenter},
            {nameof(Employee.Manager)}{nameof(Employee.Id)} = {employeeManagerId},
            {nameof(Employee.PrimaryApprover)}{nameof(Employee.Id)} = {employeePrimaryApproverId},
            {nameof(Employee.SecondaryApprover)}{nameof(Employee.Id)} = {employeeSecondaryApproverId},
            {nameof(EmployeeEmploymentData.JobTitle)} = {employeeJobTitle},
            {nameof(EmployeeEmploymentData.Department)} = {employeeDepartment},
            {nameof(EmployeeEmploymentData.EmploymentDate)} = {employeeEmploymentDate},
            {nameof(EmployeeEmploymentData.IsSalaried)} = {employeeIsSalaried},
            {nameof(EmployeeEmploymentData.IsAdministrator)} = {employeeIsAdministrator},
            {nameof(Employee.IsActive)} = {employeeIsActive},
            {nameof(Employee.UsesTimesheet)} = {employeeUsesTimesheet},
            {nameof(EmployeeContactData.CompanyEmail)} = {employeeCompanyEmail},
            {nameof(EmployeeContactData.CompanyPhone)} = {employeeCompanyPhone},
            {nameof(Employee.ModifiedDate)} = {employeeModifiedDate},
            {nameof(Employee.UpdatedBy)} = {employeeUpdatedBy},
            {nameof(Employee.UserId)} = {employeeUserId}
            ";

            var insertColums = $@"
            {nameof(Employee.Id)},
            {nameof(Employee.FullName)},
            {nameof(Employee.DefaultProfitCenter)},
            {nameof(Employee.Manager)}{nameof(Employee.Id)},
            {nameof(Employee.PrimaryApprover)}{nameof(Employee.Id)},
            {nameof(Employee.SecondaryApprover)}{nameof(Employee.Id)},
            {nameof(EmployeeEmploymentData.JobTitle)},
            {nameof(EmployeeEmploymentData.Department)},
            {nameof(EmployeeEmploymentData.EmploymentDate)},
            {nameof(EmployeeEmploymentData.IsSalaried)},
            {nameof(EmployeeEmploymentData.IsAdministrator)},
            {nameof(Employee.IsActive)},
            {nameof(Employee.UsesTimesheet)},
            {nameof(EmployeeContactData.CompanyEmail)},
            {nameof(EmployeeContactData.CompanyPhone)},
            {nameof(Employee.CreatedDate)},
            {nameof(Employee.ModifiedDate)},
            {nameof(Employee.UpdatedBy)},
            {nameof(Employee.UserId)}
            ";

            var insertValues = $@"
                {employeeId},
                {employeeFullName},
                {employeeDefaultProfitCenter},
                {employeeManagerId},
                {employeePrimaryApproverId},
                {employeeSecondaryApproverId},
                {employeeJobTitle},
                {employeeDepartment},
                {employeeEmploymentDate},
                {employeeIsSalaried},
                {employeeIsAdministrator},
                {employeeIsActive},
                {employeeUsesTimesheet},
                {employeeCompanyEmail},
                {employeeCompanyPhone},
                {employeeCreatedDate},
                {employeeModifiedDate},
                {employeeUpdatedBy},
                {employeeUserId}
            ";

            var query = $@"IF EXISTS (SELECT * FROM {EmployeeTable} WHERE {nameof(Employee.Id)} = {employeeId})
                         BEGIN
                             UPDATE {EmployeeTable}
                             SET {updates}
                             WHERE {nameof(Employee.Id)} = {employeeId};
                                    END
                                    ELSE
                         BEGIN
                             INSERT INTO {EmployeeTable} ({insertColums})
                             SELECT {insertValues}
                         END";

            await _databaseService.ExecuteAsync(query, new { 
                employeeId = employee.Id,
                employeeFullName = employee.FullName,
                employeeDefaultProfitCenter = employee.DefaultProfitCenter,
                employeeManagerId = employee.PrimaryApprover?.Id,
                employeePrimaryApproverId = employee.PrimaryApprover?.Id,
                employeeSecondaryApproverId = employee.SecondaryApprover?.Id,
                employeeJobTitle = employee.EmploymentData.JobTitle,
                employeeDepartment = employee.EmploymentData.Department,
                employeeEmploymentDate = employee.EmploymentData.EmploymentDate,
                employeeIsSalaried = employee.EmploymentData.IsSalaried,
                employeeIsAdministrator = employee.EmploymentData.IsAdministrator,
                employeeIsActive = employee.IsActive,
                employeeUsesTimesheet = employee.UsesTimesheet,
                employeeCompanyEmail = employee.Contacts.CompanyEmail,
                employeeCompanyPhone = employee.Contacts.CompanyPhone,
                employeeCreatedDate = employee.CreatedDate,
                employeeModifiedDate = employee.ModifiedDate,
                employeeUpdatedBy = employee.UpdatedBy,
                employeeUserId = employee.UserId
            });
        }
    }
}

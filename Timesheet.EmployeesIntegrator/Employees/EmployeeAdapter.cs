﻿using Timesheet.Domain.Models.Employees;

namespace Timesheet.EmployeesIntegrator.Employees
{
    internal class EmployeeAdapter
    {
        Employee AdaptEmployeeRecord(EmployeeRecord record)
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
                isAdministrator: record.JobRole == "ADMIN"
            );

            var employeeContactData = new EmployeeContactData(companyEmail: record.Email, companyPhone:record.Phone);

            var employee = new Employee(
                record.EmployeeCode,
                record.FullName,
                record.ManagerId,
                record.ManagerId,
                record.SecondaryApproverId,
                employmentData,
                employeeContactData,
                record.Active
            );

            return employee;
        }
    }
}

﻿using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.TImesheets.CommandHandlers;
using Timesheet.Application.Workflow;
using Timesheet.Domain;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.CommandHandlers
{
    internal class SubmitTimesheetCommandHandler : BaseTimesheetCommandHandler<TimesheetHeader, SubmitTimesheet>
    {
        private readonly IWorkflowService _workflowService;

        public SubmitTimesheetCommandHandler(
            IAuditHandler auditHandler,
            IEmployeeReadRepository employeeReadRepository,
            ITimesheetReadRepository readRepository, 
            IWorkflowService workflowService,
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitations)
            : base(auditHandler, employeeReadRepository, readRepository, dispatcher, unitOfWork, employeeHabilitations)
        {
            this._workflowService = workflowService;
        }

        public async override Task<IEnumerable<IDomainEvent>> HandleCoreAsync(SubmitTimesheet command, CancellationToken token)
        {
            var employee = await RequireEmployee(command.EmployeeId);

            var timesheet = await RequireTimesheet(command.TimesheetId, employee?.Id);
            this.RelatedAuditableEntity = timesheet;

            var timesheetEntryRef = GetTimesheetFirstData(timesheet);

            EmployeeRoleOnData currentEmployeeRoleOnData = await GetCurrentEmployeeRoleOnData(command, employee);
            _workflowService.AuthorizeTransition(timesheet, TimesheetTransitions.SUBMIT, timesheet.Status, currentEmployeeRoleOnData);
            if(timesheetEntryRef is not null)
            {
                _workflowService.AuthorizeTransition(timesheetEntryRef, TimesheetEntryTransitions.SUBMIT, timesheetEntryRef.Status, currentEmployeeRoleOnData);
            }

            timesheet.Submit(employee, command.Comment);

            var events = timesheet.GetDomainEvents();
            timesheet.ClearDomainEvents();

            return events;
        }
    }
}

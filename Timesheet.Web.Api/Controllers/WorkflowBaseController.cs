using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Models.Employees;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    public abstract class WorkflowBaseController: BaseController
    {
        private readonly IQueryEmployee _employeeQuery;
        private readonly IWorkflowService _workflowService;
        private readonly IEmployeeHabilitation _habilitations;

        public WorkflowBaseController(
            IQueryEmployee employeeQuery,
            IWorkflowService workflowService,
            IEmployeeHabilitation habilitations)
        {
            _employeeQuery = employeeQuery;
            _workflowService = workflowService;
            _habilitations = habilitations;
        }

        public async Task<WithHabilitations<T>> SetAuthorizedTransitions<T>(T data, Type entityType, Enum status, User currentUser, string employeeId)
        {
            var dataWithHabilitations = new WithHabilitations<T>(data);
            if(data is null)
            {
                return dataWithHabilitations;
            }

            var employee = await _employeeQuery.GetEmployeeApprovers(employeeId);
            var authorRole = _habilitations.GetEmployeeRoleOnData(currentUser.Id, 
                currentUser.IsAdministrator,
                employeeId,
                employee.PrimaryApproverId,
                employee.SecondaryApproverId);

            var authorizedTransitions = _workflowService.NextTranstitions(entityType, status, authorRole);

            if(authorizedTransitions is not null && authorizedTransitions.Any())
            {
                dataWithHabilitations.AuthorizedActions = authorizedTransitions
                    .Select(a => new AuthorizedAction(Convert.ToInt32(a), a.ToString())).ToList();
            }

            return dataWithHabilitations;
        }


        public async Task<WithHabilitations<U>> CombineAuthorizedTransitions<U, T>(WithHabilitations<U> currentDataWithHabilitations, 
            T data, Type entityType, Enum status, User currentUser, string employeeId)
        {
            var dataWithHabilitation = await SetAuthorizedTransitions(data, entityType, status, currentUser, employeeId);
                
            currentDataWithHabilitations.AuthorizedActions = currentDataWithHabilitations.AuthorizedActions
                .Union(dataWithHabilitation.AuthorizedActions)
                .ToList();

            return currentDataWithHabilitations;
        }
    }
}

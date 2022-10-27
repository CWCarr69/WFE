using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Employees.Services;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : WorkflowBaseController
    {
        private readonly IQueryEmployee _employeeQuery;
        private readonly IEmployeeBenefitCalculator _benefitsServcies;

        public EmployeeController(IQueryEmployee employeeQuery,
            IWorkflowService workflowService,
            IEmployeeHabilitation employeeHabilitation,
            IEmployeeBenefitCalculator benefitsServcies)
            :base(employeeQuery, workflowService, employeeHabilitation)
        {
            _employeeQuery = employeeQuery;
            this._benefitsServcies = benefitsServcies;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeProfile>>> Get()
        {
            var employee = await _employeeQuery.GetEmployees();
            return Ok(employee);
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeProfile>> Get(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeProfile(employeeId);
            return Ok(employee);
        }

        [HttpGet("{employeeId}/Approvers")]
        public async Task<ActionResult<EmployeeApprovers>> GetApprovers(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeApprovers(employeeId);
            return Ok(employee);
        }

        [HttpGet("{employeeId}/Benefits")]
        public async Task<ActionResult<EmployeeBenefits>> GetBenefits(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeProfile(employeeId);
            if(employee?.Id is null || employee?.EmploymentDate is null)
            {
                return BadRequest("Employee not found");
            }
            var employeeBenefits = await _benefitsServcies.GetBenefits(employeeId, employee.EmploymentDate.Value);
            return Ok(employeeBenefits);
        }

        [HttpGet("Team")]
        public async Task<ActionResult<PaginatedResult<EmployeeWithTimeStatus>>> GetTeamTimeRecordStatus(bool directReport, int page = 1, int itemsPerpage = 50)
        {
            string managerId = Manager();
            var employeeTeam = await _employeeQuery.GetEmployeeTeam(page, itemsPerpage, managerId, directReport);

            var result = new PaginatedResult<EmployeeWithTimeStatus>
            {
                Page = page,
                ItemsPerPage = itemsPerpage,
                TotalItems = employeeTeam.TotalItems,
                Items = employeeTeam.Employees
            };

            return Ok(result);
        }

        [HttpGet("Timeoff/Pending")]
        public async Task<ActionResult<IEnumerable<WithHabilitations<EmployeeTimeoff>>>> GetTeamPendingTimeoffs(bool directReport)
        {
            string managerId = Manager();
            var timeoffs = await _employeeQuery.GetEmployeesPendingTimeoffs(managerId, directReport);
            
            var timeoffWithHabilitations = new List<WithHabilitations<EmployeeTimeoff>>();
            foreach (var timeoff in timeoffs)
            {
                var timeoffWithHabilitation = await SetAuthorizedTransitions(timeoff, typeof(TimeoffHeader), timeoff.Status, CurrentUser, timeoff.EmployeeId);
                timeoffWithHabilitations.Add(timeoffWithHabilitation);
            }
            return Ok(timeoffWithHabilitations);
        }

        [HttpGet("Timesheet/Pending")]
        public async Task<ActionResult<IEnumerable<WithHabilitations<EmployeeTimesheet>>>> GetTeamPendingTimesheets(bool directReport)
        {
            string managerId = Manager();
            var timesheets = await _employeeQuery.GetEmployeesPendingTimesheets(managerId, directReport);

            var timesheetWithHabilitations = new List<WithHabilitations<EmployeeTimesheet>>();
            foreach (var timesheet in timesheets)
            {
                var timesheetWithHabilitation = await SetAuthorizedTransitions(timesheet, typeof(TimesheetHeader), timesheet.Status, CurrentUser, timesheet.EmployeeId);
                timesheetWithHabilitations.Add(timesheetWithHabilitation);
            }
            return Ok(timesheetWithHabilitations);
        }

        private string Manager()
        {
            return CurrentUser.IsAdministrator ? null : CurrentUser.Id;
        }

    }
}

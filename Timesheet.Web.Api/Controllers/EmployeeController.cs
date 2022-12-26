using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Holidays.Commands;
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
    public class EmployeeController : WorkflowBaseController<EmployeeController>
    {
        private readonly IQueryEmployee _employeeQuery;
        private readonly IEmployeeBenefitCalculator _benefitsServcies;
        private readonly IDispatcher _dispatcher;


        public EmployeeController(IQueryEmployee employeeQuery,
            IDispatcher dispatcher,
            IWorkflowService workflowService,
            IEmployeeHabilitation employeeHabilitation,
            IEmployeeBenefitCalculator benefitsServcies,
            ILogger<EmployeeController> logger)
            :base(employeeQuery, workflowService, employeeHabilitation, logger)
        {
            _employeeQuery = employeeQuery;
            this._benefitsServcies = benefitsServcies;
            this._dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeProfile>>> Get()
        {
            LogInformation("Getting Employees");

            var employee = await _employeeQuery.GetEmployees();
            return Ok(employee);
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeProfile>> Get(string employeeId, bool withApprovers = false)
        {
            LogInformation($"Getting Employee ({employeeId}) Details");

            var employee = await _employeeQuery.GetEmployeeProfile(employeeId, withApprovers);
            return Ok(employee);
        }

        [HttpGet("{employeeId}/Approvers")]
        public async Task<ActionResult<EmployeeApprovers>> GetApprovers(string employeeId)
        {
            LogInformation($"Getting Employee ({employeeId}) Approvers");

            var employee = await _employeeQuery.GetEmployeeApprovers(employeeId);
            return Ok(employee);
        }

        [HttpGet("{employeeId}/CalculatedBenefits")]
        public async Task<ActionResult<EmployeeCalculatedBenefits>> GetCalculatedBenefits(string employeeId)
        {
            LogInformation($"Getting Employee ({employeeId}) Calculated Benefits");

            var employee = await _employeeQuery.GetEmployeeProfile(employeeId);
            if(employee?.Id is null || employee?.EmploymentDate is null)
            {
                return BadRequest("Employee not found");
            }
            var employeeBenefits = await _benefitsServcies.GetBenefits(employeeId, employee.EmploymentDate.Value);
            return Ok(employeeBenefits);
        }

        [HttpGet("{employeeId}/Benefits")]
        public async Task<ActionResult<EmployeeCalculatedBenefits>> GetBenefits(string employeeId)
        {
            LogInformation($"Getting Employee ({employeeId}) Benefits");

            var employeeBenefits = await _employeeQuery.GetEmployeeBenefitsVariation(employeeId);
            if (employeeBenefits is null)
            {
                return BadRequest("Employee not found");
            }
            return Ok(employeeBenefits);
        }

        [HttpGet("Team")]
        public async Task<ActionResult<PaginatedResult<EmployeeWithTimeStatus>>> GetTeamRecordStatus(bool directReport, int page = 1, int itemsPerPage = 10000)
        {
            string managerId = Manager();

            LogInformation($"Getting Employee ({managerId}) Team");

            var employeeTeam = await _employeeQuery.GetEmployeeTeam(page, itemsPerPage, managerId, directReport);

            return Ok(Paginate(page, itemsPerPage, employeeTeam));
        }

        [HttpGet("Timeoff/Pending")]
        public async Task<ActionResult<PaginatedResult<WithHabilitations<EmployeeTimeoff>>>> GetTeamPendingTimeoffs(bool directReport, int page = 1, int itemsPerPage = 10000)
        {
            string managerId = Manager();
            LogInformation($"Getting Employee ({managerId}) Pending timeoffs");

            var timeoffs = await _employeeQuery.GetEmployeesPendingTimeoffs(page, itemsPerPage, managerId, directReport);
            
            var timeoffWithHabilitations = new List<WithHabilitations<EmployeeTimeoff>>();
            foreach (var timeoff in timeoffs.Items)
            {
                var timeoffWithHabilitation = await SetAuthorizedTransitions(timeoff, typeof(TimeoffHeader), timeoff.Status, CurrentUser, timeoff.EmployeeId);
                timeoffWithHabilitations.Add(timeoffWithHabilitation);
            }

            return Ok(Paginate(page, itemsPerPage, timeoffs.TotalItems, timeoffWithHabilitations));
        }

        [HttpGet("Timesheet/Pending")]
        public async Task<ActionResult<IEnumerable<WithHabilitations<EmployeeTimesheet>>>> GetTeamPendingTimesheets(bool directReport, int page = 1, int itemsPerPage = 10000)
        {
            string managerId = Manager();
            LogInformation($"Getting Employee ({managerId}) Pending timesheets");

            var timesheets = await _employeeQuery.GetEmployeesPendingTimesheets(page, itemsPerPage, managerId, directReport);

            var timesheetWithHabilitations = new List<WithHabilitations<EmployeeTimesheet>>();
            foreach (var timeoff in timesheets.Items)
            {
                var timesheetWithHabilitation = await SetAuthorizedTransitions(timeoff, typeof(TimesheetHeader), timeoff.Status, CurrentUser, timeoff.EmployeeId);
                timesheetWithHabilitations.Add(timesheetWithHabilitation);
            }

            return Ok(Paginate(page, itemsPerPage, timesheets.TotalItems, timesheetWithHabilitations));
        }


        [HttpPut("{employeeId}/Benefits")]
        public async Task<IActionResult> SetBenefits([FromBody] ModifyEmployeeBenefits modifyEmployeeBenefits, CancellationToken token)
        {
            LogInformation($"Set Employee ({modifyEmployeeBenefits.EmployeeId})");

            await _dispatcher.RunCommand(modifyEmployeeBenefits, CurrentUser, token);

            LogInformation($"Employee Benefits set");
            return Ok();
        }

        private string Manager()
        {
            return CurrentUser.IsAdministrator ? null : CurrentUser.Id;
        }

    }
}

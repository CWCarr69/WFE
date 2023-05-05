using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Timesheet.Application;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Application.Timesheets.Services;
using Timesheet.Application.Timesheets.Services.Export;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class TimesheetController : WorkflowBaseController<TimesheetController>
    {
        private const TimesheetTransitions FINALIZE_TRANSITION = TimesheetTransitions.FINALIZE;
        private readonly IQueryTimesheet _timesheetQuery;
        private readonly IQueryEmployee _employeeQuery;
        private readonly IDispatcher _dispatcher;
        private readonly IExportTimesheetService _exportTimesheet;
        private readonly ITimesheetPeriodService _periodService;

        public TimesheetController(
            IQueryTimesheet timesheetQuery,
            IDispatcher dispatcher,
            IQueryEmployee employeeQuery,
            IWorkflowService workflowService,
            IEmployeeHabilitation habilitations,
            IExportTimesheetService exportTimesheet,
            ITimesheetPeriodService periodService,
            ILogger<TimesheetController> logger
            )
            : base(employeeQuery, workflowService, habilitations, logger)
        {
            this._employeeQuery = employeeQuery;
            this._timesheetQuery = timesheetQuery;
            this._dispatcher = dispatcher;
            this._exportTimesheet = exportTimesheet;
            this._periodService = periodService;
        }

        [HttpGet("History/Employee/{employeeId}")]
        public async Task<ActionResult<PaginatedResult<EmployeeTimesheet>>> GetTimesheetHistory(string employeeId, int page = 1, int itemsPerpage = 10000)
        {
            LogInformation($"Getting Employee ({employeeId}) Timesheet history");
            
            var timesheets = await _timesheetQuery.GetEmployeeTimesheets(employeeId, page, itemsPerpage);
            return Ok(Paginate(page, itemsPerpage, timesheets));
        }

        [HttpGet("History/Entries/Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeTimesheet>>> GetTimesheetHistoryEntries(string employeeId, DateTime start)
        {
            LogInformation($"Getting Employee ({employeeId}) Timesheet history entries");

            var monthStart = new DateTime(start.Year, start.Month, 1);
            var monthEnd = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));

            var timesheets = await _timesheetQuery.GetEmployeeTimesheetEntriesInPeriod(employeeId, monthStart, monthEnd);
            return Ok(timesheets);
        }

        [HttpGet("{timesheetId}/Employee/{employeeId}")]
        public async Task<ActionResult<WithHabilitations<EmployeeTimesheet>>> GetTimesheetDetails(string employeeId, string timesheetId)
        {
            LogInformation($"Getting Employee ({employeeId}) Timesheet ({timesheetId}) Details");
            
            var timesheet = await _timesheetQuery.GetEmployeeTimesheetDetails(employeeId, timesheetId);
            var response = await SetAuthorizedTransitions(employeeId, timesheet);
            return Ok(response);
        }

        [HttpGet("{timesheetId}/SummaryByDate/Employee/{employeeId}")]
        public async Task<ActionResult<EmployeeTimesheetDetailSummary>> GetTimesheetSummaryByDate(string employeeId, string timesheetId)
        {
            LogInformation($"Getting Employee ({employeeId}) Timesheet ({timesheetId}) Summary by date");

            var timeoffs = await _timesheetQuery.GetEmployeeTimesheetSummaryByDate(employeeId, timesheetId);
            return Ok(timeoffs);
        }

        [HttpGet("CurrentTimesheetPeriod/Employee/{employeeId}")]
        public async Task<ActionResult<TimesheetPeriod>> GetCurrentTimesheet(string employeeId)
        {
            LogInformation($"Getting Employee ({employeeId}) current period");

            var employee = await _employeeQuery.GetEmployeeProfile(employeeId);

            if (employee?.Id is null)
            {
                return BadRequest("Employee not found");
            }

            return Ok(_periodService.GetCurrentPeriod(employee.IsSalaried));
        }

        [HttpGet("{timesheetId}/SummaryByPayroll/Employee/{employeeId}")]
        public async Task<ActionResult<EmployeeTimesheetDetailSummary>> GetTimesheetSummaryByPayroll(string employeeId, string timesheetId)
        {
            LogInformation($"Getting Employee ({employeeId}) Timesheet ({timesheetId}) Summary by payroll code");

            var timeoffs = await _timesheetQuery.GetEmployeeTimesheetSummaryByPayrollCode(employeeId, timesheetId);
            return Ok(timeoffs);
        }

        [HttpGet("Review")]
        public async Task<ActionResult<WithHabilitations<PaginatedResult<WithHabilitations<EmployeeTimesheetWithTotals>>>>> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page=1, int itemsPerpage=10000)
        {
            LogInformation($"Getting Timesheet ({payrollPeriod}) review");
            string managerId = Manager();

            var timesheetReview = await _timesheetQuery.GetTimesheetReview(payrollPeriod, employeeId, department, page, itemsPerpage, managerId);

            var reviewWithHabilitations = new List<WithHabilitations<EmployeeTimesheetWithTotals>>();
            foreach(var review in timesheetReview.Items)
            {
                var reviewWithHabilitation = await SetAuthorizedTransitions(review.EmployeeId, review);
                reviewWithHabilitations.Add(reviewWithHabilitation);
            }

            var payrollPeriodEnd = reviewWithHabilitations.FirstOrDefault()?.Data?.EndDate;
            var isFinalizable = DateTime.Now >= payrollPeriodEnd;
            var globalTimesheetAuthorizedTranstions = isFinalizable 
                ? reviewWithHabilitations
                .FirstOrDefault()?
                .AuthorizedActions?
                .Where(a => a.Value.Equals((int)TimesheetTransitions.FINALIZE))
                .ToList()
                : null;

            var result = Paginate(page, itemsPerpage, timesheetReview.TotalItems, reviewWithHabilitations);
            result.OtherData.Add(nameof(TimesheetReview.TotalQuantity), timesheetReview.TotalQuantity);

            var resultWithHabilitations = new WithHabilitations<PaginatedResult<WithHabilitations<EmployeeTimesheetWithTotals>>> (result, globalTimesheetAuthorizedTranstions);

            return Ok(resultWithHabilitations);
        }

        [AllowAnonymous]
        [HttpGet("{payrollPeriod}/Export")]
        public async Task<IActionResult> ExportTimesheet(string payrollPeriod, string? department = null, string? employeeId = null)
        {
            var csvData = await _exportTimesheet.ExportRawReviewToWeb(payrollPeriod, department, employeeId);
            var filesBytes = Encoding.UTF8.GetBytes(csvData);

            return File(filesBytes, "text/csv", $"Timesheet_{payrollPeriod}.csv");
        }

        [AllowAnonymous]
        [HttpGet("{payrollPeriod}/Export/AfterFinalize")]
        public async Task<IActionResult> ExportTimesheetAfterFinalize(string payrollPeriod)
        {
            var csvData = await _exportTimesheet.ExportAdaptedReviewToWeb(payrollPeriod);
            var filesBytes = Encoding.UTF8.GetBytes(csvData);

            return File(filesBytes, "text/csv", $"Timesheet_external_{payrollPeriod}.csv");
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPost("Entries")]
        public async Task<IActionResult> AddEntry([FromBody] AddTimesheetEntry command, CancellationToken token)
        {
            LogInformation($"Adding timesheet entry for employee ({command.EmployeeId} on date {command.WorkDate})");

            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Timesheet entry added for employee ({command.EmployeeId} on date {command.WorkDate})");
            return Ok();
        }

        [HttpPut("AddComment")]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateTimesheetComment command, CancellationToken token)
        {
            LogInformation($"Updatting comment on timesheet ({command.TimesheetId}) for employee ({command.EmployeeId})");

            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Comment is updated on timesheet ({command.TimesheetId}) for employee ({command.EmployeeId})");
            return Ok();
        }

        [HttpDelete("Entries")]
        public async Task<IActionResult> DeleteEntry([FromBody] DeleteTimesheetEntry command, CancellationToken token)
        {
            LogInformation($"Deleting timesheet entry ({command.TimesheetEntryId}) for employee ({command.EmployeeId})");

            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Timesheet entry ({command.TimesheetEntryId}) for employee ({command.EmployeeId}) deleted");
            return Ok();
        }

        [HttpPut("Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTimesheet command, CancellationToken token)
        {
            LogInformation($"Submiting timesheet ({command.TimesheetId}-{command.EmployeeId})");
            
            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Timesheet ({command.TimesheetId}-{command.EmployeeId}) Updated");
            return Ok();
        }

        [Authorize(Roles = "MANAGER, ADMINISTRATOR")]
        [HttpPut("Approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveTimesheet command, CancellationToken token)
        {
            LogInformation($"Approving timesheet ({command.TimesheetId}-{command.EmployeeId})");
            
            await _dispatcher.RunCommand(command, CurrentUser, token);
            
            LogInformation($"Timesheet ({command.TimesheetId}-{command.EmployeeId}) Updated");
            return Ok();
        }


        [Authorize(Roles = "MANAGER, ADMINISTRATOR")]
        [HttpPut("Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectTimesheet command, CancellationToken token)
        {
            LogInformation($"Rejecting timesheet ({command.TimesheetId}-{command.EmployeeId})");
            
            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Timesheet ({command.TimesheetId}-{command.EmployeeId}) Updated");
            return Ok();
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPut("Finalize")]
        public async Task<IActionResult> Finalize([FromBody] FinalizeTimesheet command, CancellationToken token)
        {
            LogInformation($"Finalize timesheet ({command.TimesheetId})");
            
            await _dispatcher.RunCommand(command, CurrentUser, token);
            
            LogInformation($"Timesheet ({command.TimesheetId}) Updated");
            return Ok();
        }

        [HttpPost("Exceptions")]
        public async Task<IActionResult> AddExceptions([FromBody] AddTimesheetException command, CancellationToken token)
        {
            LogInformation($"Adding timesheet exception for employee ({command.EmployeeId}) on Timesheet ({command.TimesheetId})");

            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Exception added to timesheet {command.TimesheetId}) for employee ({command.EmployeeId})");
            return Ok();
        }

        private async Task<WithHabilitations<EmployeeTimesheet>> SetAuthorizedTransitions(string employeeId, EmployeeTimesheet? timesheet)
        {
            var timesheetEntry = timesheet?.EntriesWithoutTimeoffs?.OrderBy(t => t.Status).ToList().FirstOrDefault(t => t.Status != TimesheetEntryStatus.APPROVED);
           
            return await SetAuthorizedTransitions(employeeId, timesheet, timesheetEntry);
        }

        private async Task<WithHabilitations<EmployeeTimesheetWithTotals>> SetAuthorizedTransitions(string employeeId, EmployeeTimesheetWithTotals? timesheet)
        {
            var timesheetEntry = timesheet?.Entries?.OrderBy(t => t.Status).ToList().FirstOrDefault();
            return await SetAuthorizedTransitions(employeeId, timesheet, timesheetEntry);
        }

        private async Task<WithHabilitations<T>> SetAuthorizedTransitions<T>(string employeeId, T timesheet, dynamic timesheetEntry)
        {
            var hasFinalizeAction = false;
            dynamic dynamicTimesheet = timesheet;
            WithHabilitations<T> response = await SetAuthorizedTransitions(timesheet, typeof(TimesheetHeader), dynamicTimesheet?.Status, CurrentUser, employeeId);
            if(response.AuthorizedActions.Any(a => a.Name == FINALIZE_TRANSITION.ToString())){
                hasFinalizeAction = true;
            }

            response = await CombineIntersectAuthorizedTransitions(response, timesheetEntry, typeof(TimesheetEntry), timesheetEntry?.Status, CurrentUser, employeeId);

            if (hasFinalizeAction)
            {
                response.AuthorizedActions = response.AuthorizedActions.Union(
                new List<AuthorizedAction>()
                {
                    new AuthorizedAction((int) FINALIZE_TRANSITION, FINALIZE_TRANSITION.ToString())
                }).ToList();
            }
            
            return response;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Timesheets.Commands;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    [ApiController]
    public class TimesheetController : WorkflowBaseController
    {
        private readonly IQueryTimesheet _timesheetQuery;
        private readonly IDispatcher _dispatcher;

        public TimesheetController(
            IQueryTimesheet timesheetQuery,
            IDispatcher dispatcher,
            IQueryEmployee employeeQuery,
            IWorkflowService workflowService,
            IEmployeeHabilitation habilitations)
            : base(employeeQuery, workflowService, habilitations)
        {
            this._timesheetQuery = timesheetQuery;
            this._dispatcher = dispatcher;
        }

        [HttpGet("History/Employee/{employeeId}")]
        public async Task<ActionResult<PaginatedResult<EmployeeTimesheet>>> GetTimesheetHistory(string employeeId, int page = 1, int itemsPerpage = 50)
        {
            var timesheets = await _timesheetQuery.GetEmployeeTimesheets(employeeId, page, itemsPerpage);
            return Ok(Paginate(page, itemsPerpage, timesheets));
        }

        [HttpGet("{timesheetId}/Employee/{employeeId}")]
        public async Task<ActionResult<WithHabilitations<EmployeeTimesheet>>> GetTimesheetDetails(string employeeId, string timesheetId)
        {
            var timesheet = await _timesheetQuery.GetEmployeeTimesheetDetails(employeeId, timesheetId);
            var response = await SetAuthorizedTransitions(employeeId, timesheet);
            return Ok(response);
        }

        [HttpGet("{timesheetId}/SummaryByDate/Employee/{employeeId}")]
        public async Task<ActionResult<EmployeeTimesheetDetailSummary>> GetTimesheetSummaryByDate(string employeeId, string timesheetId)
        {
            var timeoffs = await _timesheetQuery.GetEmployeeTimesheetSummaryByDate(employeeId, timesheetId);
            return Ok(timeoffs);
        }

        [HttpGet("{timesheetId}/SummaryByPayroll/Employee/{employeeId}")]
        public async Task<ActionResult<EmployeeTimesheetDetailSummary>> GetTimesheetSummaryByPayroll(string employeeId, string timesheetId)
        {
            var timeoffs = await _timesheetQuery.GetEmployeeTimesheetSummaryByPayrollCode(employeeId, timesheetId);
            return Ok(timeoffs);
        }

        [HttpGet("Review")]
        public async Task<ActionResult<PaginatedResult<TimesheetReview>>> GetTimesheetReview(string payrollPeriod, string? employeeId, string? department, int page=1, int itemsPerpage=50)
        {
            var timesheetReview = await _timesheetQuery.GetTimesheetReview(payrollPeriod, employeeId, department, page, itemsPerpage);

            var reviewWithHabilitations = new List<WithHabilitations<EmployeeTimesheetWithTotals>>();
            foreach(var review in timesheetReview.Items)
            {
                var reviewWithHabilitation = await SetAuthorizedTransitions(review.EmployeeId, review);
                reviewWithHabilitations.Add(reviewWithHabilitation);
            }

            var result = Paginate(page, itemsPerpage, timesheetReview.TotalItems, reviewWithHabilitations);
            result.OtherData.Add(nameof(TimesheetReview.TotalQuantity), timesheetReview.TotalQuantity);

            return Ok(result);
        }

        [HttpPut("Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTimesheet command, CancellationToken token)
        {
            await _dispatcher.RunCommand(command, CurrentUser, token);
            return Ok();
        }

        [Authorize(Roles = "SUPERVISOR, MANAGER, ADMINISTRATOR")]
        [HttpPut("Approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveTimesheet command, CancellationToken token)
        {
            await _dispatcher.RunCommand(command, CurrentUser, token);
            return Ok();
        }


        [Authorize(Roles = "SUPERVISOR, MANAGER, ADMINISTRATOR")]
        [HttpPut("Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectTimesheet command, CancellationToken token)
        {
            await _dispatcher.RunCommand(command, CurrentUser, token);
            return Ok();
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpPut("Finalize")]
        public async Task<IActionResult> Finalize([FromBody] FinalizeTimesheet command, CancellationToken token)
        {
            await _dispatcher.RunCommand(command, CurrentUser, token);
            return Ok();
        }

        private async Task<WithHabilitations<EmployeeTimesheet>> SetAuthorizedTransitions(string employeeId, EmployeeTimesheet? timesheet)
        {
            var timesheetEntry = timesheet?.Entries?.FirstOrDefault(e =>
            e.PayrollCode != TimesheetPayrollCode.HOLIDAY.ToString()
            && e.PayrollCode != TimesheetPayrollCode.TIMEOFF.ToString()
            );
            return await SetAuthorizedTransitions(employeeId, timesheet, timesheetEntry);
        }

        private async Task<WithHabilitations<EmployeeTimesheetWithTotals>> SetAuthorizedTransitions(string employeeId, EmployeeTimesheetWithTotals? timesheet)
        {
            var timesheetEntry = timesheet?.Entries?.FirstOrDefault(e => 
            e.PayrollCode != TimesheetPayrollCode.HOLIDAY.ToString()
            && e.PayrollCode != TimesheetPayrollCode.TIMEOFF.ToString()
            );
            return await SetAuthorizedTransitions(employeeId, timesheet, timesheetEntry);
        }

        private async Task<WithHabilitations<T>> SetAuthorizedTransitions<T>(string employeeId, T timesheet, dynamic timesheetEntry)
        {
            dynamic dynamicTimesheet = timesheet;
            var response = await SetAuthorizedTransitions(timesheet, typeof(TimesheetHeader), dynamicTimesheet?.Status, CurrentUser, employeeId);
            if (timesheetEntry is not null)
            {
                response = await CombineAuthorizedTransitions(response, timesheetEntry, typeof(TimesheetEntry), timesheetEntry?.Status, CurrentUser, employeeId);
            }
            return response;
        }
    }
}

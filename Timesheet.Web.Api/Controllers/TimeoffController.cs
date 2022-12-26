using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize]
    [Route("api/Employee")]
    [ApiController]
    public class TimeoffController : WorkflowBaseController<TimeoffController>
    {
        private readonly IQueryTimeoff _timeoffQuery;
        private readonly IDispatcher _dispatcher;

        public TimeoffController(
            IQueryEmployee employeeQuery,
            IQueryTimeoff timeoffQuery,
            IDispatcher dispatcher,
            IWorkflowService workflowService,
            IEmployeeHabilitation habilitations, 
            ILogger<TimeoffController> logger)
            :base(employeeQuery, workflowService, habilitations, logger)
        {
            _timeoffQuery = timeoffQuery;
            _dispatcher = dispatcher;
        }

        [HttpGet("{employeeId}/Timeoff/History")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoff>>> GetTimeoffHistory(string employeeId, int page = 1, int itemsPerpage = 10000, bool requireApproval=true)
        {
            LogInformation($"Getting Employee ({employeeId}) Timeoff history");
            
            var timeoffs = await _timeoffQuery.GetEmployeeTimeoffs(employeeId, page, itemsPerpage, requireApproval);
            return Ok(Paginate(page, itemsPerpage, timeoffs));
        }

        [HttpGet("{employeeId}/Timeoff/History/Entries")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoff>>> GetTimeoffHistoryEntries(string employeeId, DateTime start, bool requireApproval = true)
        {
            LogInformation($"Getting Employee ({employeeId}) Timeoff history entries");

            var monthStart = new DateTime(start.Year, start.Month, 1);
            var monthEnd = new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));

            var timeoffs = await _timeoffQuery.GetEmployeeTimeoffEntriesInPeriod(employeeId, monthStart, monthEnd, requireApproval);
            return Ok(timeoffs);
        }

        [HttpGet("{employeeId}/Timeoff/MonthsStatistics")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoffMonthStatisticsGroupByMonth>>> GetTimeoffHistoryMonthsStatistics(string employeeId)
        {
            LogInformation($"Getting Employee ({employeeId}) Timeoff history statistics");
            
            var statistics = await _timeoffQuery.GetEmployeeTimeoffsMonthStatistics(employeeId);
            return Ok(statistics);
        }


        [HttpGet("{employeeId}/Timeoff/{timeoffId}")]
        public async Task<ActionResult<WithHabilitations<EmployeeTimeoff>>> GetTimeoff(string employeeId, string timeoffId)
        {
            LogInformation($"Getting Employee ({employeeId}) Timeoff ({timeoffId}) Details");
            
            var timeoff = await _timeoffQuery.GetEmployeeTimeoffDetails(employeeId, timeoffId);
            var response = await SetAuthorizedTransitions(timeoff, typeof(TimeoffHeader), timeoff?.Status, CurrentUser, employeeId);
            return Ok(response);
        }

        [HttpGet("{employeeId}/Timeoff/{timeoffId}/Summary")]
        public async Task<ActionResult<EmployeeTimeoffDetailSummary>> GetTimeoffSummary(string employeeId, string timeoffId)
        {
            LogInformation($"Getting Employee ({employeeId}) Timeoff ({timeoffId}) Summary");
            
            var timeoffs = await _timeoffQuery.GetEmployeeTimeoffSummary(employeeId, timeoffId);
            return Ok(timeoffs);
        }

        [HttpPost("timeoff")]
        public async Task<IActionResult> Create([FromBody] CreateTimeoff addTimeoff, CancellationToken token)
        {
            LogInformation($"Creating new Timeoff for Employee ({addTimeoff.EmployeeId})");
            
            await _dispatcher.RunCommand(addTimeoff, CurrentUser, token);

            LogInformation($"Timeoff ({addTimeoff.EmployeeId}) created");
            return Ok();
        }

        [HttpPost("timeoff/Entry")]
        public async Task<IActionResult> AddEntry([FromBody] AddEntryToTimeoff addEntryTimeoff, CancellationToken token)
        {
            LogInformation($"Adding new entry to Timeoff ({addEntryTimeoff.TimeoffId}) for Employee ({addEntryTimeoff.EmployeeId})");

            await _dispatcher.RunCommand(addEntryTimeoff, CurrentUser, token);

            LogInformation($"Timeoff ({addEntryTimeoff.TimeoffId}) updated");
            return Ok();
        }

        [HttpPut("timeoff/Entry")]
        public async Task<IActionResult> UpdateEntry([FromBody] UpdateTimeoffEntry updateTimeoffEntry, CancellationToken token)
        {
            LogInformation($"Updating entry ({updateTimeoffEntry.TimeoffEntryId}) of Timeoff ({updateTimeoffEntry.TimeoffId}) for Employee ({updateTimeoffEntry.EmployeeId})");

            await _dispatcher.RunCommand(updateTimeoffEntry, CurrentUser, token);

            LogInformation($"Timeoff ({updateTimeoffEntry.TimeoffId}) updated");
            return Ok();
        }

        [HttpPut("timeoff/{timeoffId}/AddComment")]
        public async Task<IActionResult> AddComment([FromBody] UpdateTimeoffComment updateTimeoffComment, CancellationToken token)
        {
            LogInformation($"Updating comment for Employee's ({updateTimeoffComment.EmployeeId}) Timeoff ({updateTimeoffComment.TimeoffId})");

            var command = new DeleteTimeoff() { EmployeeId = updateTimeoffComment.EmployeeId, TimeoffId = updateTimeoffComment.TimeoffId };
            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Comment updated for Timeoff ({updateTimeoffComment.TimeoffId})");
            return Ok();
        }

        [HttpDelete("timeoff/{timeoffId}")]
        public async Task<IActionResult> Delete(string employeeId, string timeoffId, CancellationToken token)
        {
            LogInformation($"Deleting Employee ({employeeId}) Timeoff ({timeoffId})");

            var command = new DeleteTimeoff() { EmployeeId = employeeId, TimeoffId = timeoffId };
            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Timeoff ({timeoffId}) Deleted");
            return Ok();
        }

        [HttpDelete("timeoff/{timeoffId}/Entry/{entryId}")]
        public async Task<IActionResult> DeleteEntry(string employeeId, string timeoffId, string entryId, CancellationToken token)
        {
            LogInformation($"Deleting entry ({entryId}) of Timeoff({timeoffId}) for Employee ({employeeId})");

            var command = new DeleteTimeoffEntry() { EmployeeId = employeeId, TimeoffId = timeoffId, TimeoffEntryId = entryId };
            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Timeoff ({timeoffId}) Updated");
            return Ok();
        }

        [HttpPut("timeoff/Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTimeoff submitTimeoff, CancellationToken token)
        {
            LogInformation($"Submiting timeoff ({submitTimeoff.TimeoffId})");

            await _dispatcher.RunCommand(submitTimeoff, CurrentUser, token);

            LogInformation($"Timeoff ({submitTimeoff.TimeoffId}) Updated");
            return Ok();
        }

        [HttpPut("timeoff/Approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveTimeoff approveTimeoff, CancellationToken token)
        {
            LogInformation($"Approving timeoff ({approveTimeoff.TimeoffId})");

            await _dispatcher.RunCommand(approveTimeoff, CurrentUser, token);

            LogInformation($"Timeoff ({approveTimeoff.TimeoffId}) Updated");
            return Ok();
        }

        [HttpPut("timeoff/Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectTimeoff rejectTimeoff, CancellationToken token)
        {
            LogInformation($"Rejecting timeoff ({rejectTimeoff.TimeoffId})");

            await _dispatcher.RunCommand(rejectTimeoff, CurrentUser, token);
            
            LogInformation($"Timeoff ({rejectTimeoff.TimeoffId}) Updated");
            return Ok();
        }
    }
}

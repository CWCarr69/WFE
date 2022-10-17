using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/Employee")]
    [ApiController]
    public class TimeoffController : ControllerBase
    {
        private readonly IQueryTimeoff _timeoffQuery;
        private readonly IDispatcher _dispatcher;

        public TimeoffController(IQueryTimeoff timeoffQuery, IDispatcher dispatcher)
        {
            _timeoffQuery = timeoffQuery;
            _dispatcher = dispatcher;
        }

        [HttpGet("{employeeId}/Timeoff/History")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoff>>> GetTimeoffHistory(string employeeId)
        {
            var timeoffs = await _timeoffQuery.GetEmployeeTimeoffs(employeeId);
            return Ok(timeoffs);
        }

        [HttpGet("{employeeId}/Timeoff/MonthsStatistics")]
        public async Task<ActionResult<EmployeeTimeoffMonthStatistics>> GetTimeoffHistoryMonthsStatistics(string employeeId)
        {
            var statistics = await _timeoffQuery.GetEmployeeTimeoffsMonthStatistics(employeeId);
            return Ok(statistics);
        }


        [HttpGet("{employeeId}/Timeoff/{timeoffId}")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoffEntry>>> GetTimeoff(string employeeId, string timeoffId)
        {
            var timeoffs = await _timeoffQuery.GetEmployeeTimeoffDetails(employeeId, timeoffId);
            return Ok(timeoffs);
        }

        [HttpGet("{employeeId}/Timeoff/{timeoffId}/Summary")]
        public async Task<ActionResult<EmployeeTimeoffDetailSummary>> GetTimeoffSummary(string employeeId, string timeoffId)
        {
            var timeoffs = await _timeoffQuery.GetEmployeeTimeoffSummary(employeeId, timeoffId);
            return Ok(timeoffs);
        }

        [HttpPost("timeoff")]
        public async Task<IActionResult> Create([FromBody] CreateTimeoff addTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(addTimeoff, token);
            return Ok();
        }

        [HttpPost("timeoff/Entry")]
        public async Task<IActionResult> AddEntry([FromBody] AddEntryToTimeoff addEntryTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(addEntryTimeoff, token);
            return Ok();
        }

        [HttpPut("timeoff/Entry")]
        public async Task<IActionResult> UpdateEntry([FromBody] UpdateTimeoffEntry updateTimeoffEntry, CancellationToken token)
        {
            await _dispatcher.RunCommand(updateTimeoffEntry, token);
            return Ok();
        }

        [HttpDelete("timeoff/{timeoffId}")]
        public async Task<IActionResult> Delete(string employeeId, string timeoffId, CancellationToken token)
        {
            var command = new DeleteTimeoff() { EmployeeId = employeeId, TimeoffId = timeoffId };
            await _dispatcher.RunCommand(command, token);
            return Ok();
        }

        [HttpDelete("timeoff/{timeoffId}/Entry/{entryId}")]
        public async Task<IActionResult> DeleteEntry(string employeeId, string timeoffId, string entryId, CancellationToken token)
        {
            var command = new DeleteTimeoffEntry() { EmployeeId = employeeId, TimeoffId = timeoffId, TimeoffEntryId = entryId };
            await _dispatcher.RunCommand(command, token);
            return Ok();
        }

        [HttpPut("timeoff/Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTimeoff submitTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(submitTimeoff, token);
            return Ok();
        }

        [HttpPut("timeoff/Approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveTimeoff approveTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(approveTimeoff, token);
            return Ok();
        }

        [HttpPut("timeoff/Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectTimeoff rejectTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(rejectTimeoff, token);
            return Ok();
        }
    }
}

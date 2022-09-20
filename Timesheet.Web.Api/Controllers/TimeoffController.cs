﻿using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Commands;
using Timesheet.ReadModel.Queries;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/Employee/{employeeId}/Timeoff")]
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTimeoff addTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(addTimeoff, token);
            return Ok();
        }

        [HttpPost("{timeoffId}")]
        public async Task<IActionResult> AddEntry([FromBody] AddEntryToTimeoff addEntryTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(addEntryTimeoff, token);
            return Ok();
        }

        [HttpPut("{timeoffId}")]
        public async Task<IActionResult> UpdateEntry([FromBody] UpdateTimeoffEntry updateTimeoffEntry, CancellationToken token)
        {
            await _dispatcher.RunCommand(updateTimeoffEntry, token);
            return Ok();
        }

        [HttpDelete("{timeoffId}")]
        public async Task<IActionResult> Delete(string employeeId, string timeoffId, CancellationToken token)
        {
            var command = new DeleteTimeoff() { EmployeeId = employeeId, TimeoffId = timeoffId };
            await _dispatcher.RunCommand(command, token);
            return Ok();
        }

        [HttpDelete("{timeoffId}/Entry/{entryId}")]
        public async Task<IActionResult> DeleteEntry(string employeeId, string timeoffId, string entryId, CancellationToken token)
        {
            var command = new DeleteTimeoffEntry() { EmployeeId = employeeId, TimeoffId = timeoffId, TimeoffEntryId = entryId };
            await _dispatcher.RunCommand(command, token);
            return Ok();
        }

        [HttpPut("{timeoffId}/Submit")]
        public async Task<IActionResult> Submit([FromBody] SubmitTimeoff submitTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(submitTimeoff, token);
            return Ok();
        }

        [HttpPut("{timeoffId}/Approve")]
        public async Task<IActionResult> Approve([FromBody] ApproveTimeoff approveTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(approveTimeoff, token);
            return Ok();
        }

        [HttpPut("{timeoffId}/Reject")]
        public async Task<IActionResult> Reject([FromBody] RejectTimeoff rejectTimeoff, CancellationToken token)
        {
            await _dispatcher.RunCommand(rejectTimeoff, token);
            return Ok();
        }
    }
}
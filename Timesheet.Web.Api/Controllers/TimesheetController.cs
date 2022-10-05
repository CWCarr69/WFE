using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly IQueryTimesheet _timesheetQuery;

        public TimesheetController(IQueryTimesheet timesheetQuery)
        {
            this._timesheetQuery = timesheetQuery;
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeTimesheet>>> GetTimesheetHistory(string? requestedEmployeeId)
        {
            var employeeId = requestedEmployeeId ?? "TO_CHANGE";
            var timesheets = await _timesheetQuery.GetEmployeeTimesheets(employeeId);
            return Ok(timesheets);
        }

        [HttpGet("{timesheetId}")]
        public async Task<ActionResult<IEnumerable<EmployeeTimesheetDetail>>> GetTimesheetDetails(string timesheetId)
        {
            var timesheet = await _timesheetQuery.GetEmployeeTimesheetDetails(timesheetId);
            return Ok(timesheet);
        }

        [HttpGet("{timesheetId}/Summary")]
        public async Task<ActionResult<EmployeeTimesheetDetailSummary>> GetTimesheetSummary(string timesheetId)
        {
            var timeoffs = await _timesheetQuery.GetEmployeeTimeoffSummary(timesheetId);
            return Ok(timeoffs);
        }

        [HttpGet("timesheetReview")]
        public async Task<ActionResult<TimeSheetReview>> GetTimesheetReview(string? payrollPeriod, string? employeeId, string? department, IEnumerable<TimesheetStatus> statuses)
        {
            var timesheetReview = await _timesheetQuery.GetEmployeeTimesheetReview(payrollPeriod, employeeId, department, statuses);
            return Ok(timesheetReview);
        }
    }
}

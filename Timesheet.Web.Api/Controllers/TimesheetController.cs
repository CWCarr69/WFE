using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TimesheetController : ControllerBase
    {
        private readonly IQueryTimesheet _timesheetQuery;

        public TimesheetController(IQueryTimesheet timesheetQuery)
        {
            this._timesheetQuery = timesheetQuery;
        }

        [HttpGet("History/Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeTimesheet>>> GetTimesheetHistory(string employeeId)
        {
            var timesheets = await _timesheetQuery.GetEmployeeTimesheets(employeeId);
            return Ok(timesheets);
        }

        [HttpGet("{timesheetId}/Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeTimesheetEntry>>> GetTimesheetDetails(string timesheetId)
        {
            var timesheet = await _timesheetQuery.GetEmployeeTimesheetDetails(timesheetId);
            return Ok(timesheet);
        }

        [HttpGet("{timesheetId}/Summary/Employee/{employeeId}")]
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

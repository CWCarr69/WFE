using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;
using Timesheet.Web.Api.ViewModels;

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
        public async Task<ActionResult<IEnumerable<EmployeeTimesheetEntry>>> GetTimesheetDetails(string employeeId, string timesheetId)
        {
            var timesheet = await _timesheetQuery.GetEmployeeTimesheetDetails(employeeId, timesheetId);
            return Ok(timesheet);
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
            var timeoffs = await _timesheetQuery.GetEmployeeTimesheetSummaryByPayroll(employeeId, timesheetId);
            return Ok(timeoffs);
        }

        [HttpGet("timesheetReview")]
        public async Task<ActionResult<PaginatedResult<TimesheetReview>>> GetTimesheetReview(string? payrollPeriod, string? employeeId, string? department, int page=1, int itemsPerpage=50)
        {
            var timesheetReview = await _timesheetQuery.GetEmployeeTimesheetReview(payrollPeriod, employeeId, department, page, itemsPerpage);
            var result = new PaginatedResult<TimesheetDetailsGroupedByEmployee>
            {
                Page = page,
                ItemsPerPage = itemsPerpage,
                TotalItems = timesheetReview.TotalItems,
                Items = timesheetReview.DetailsByEmployee,
            };
            result.OtherData.Add(nameof(TimesheetReview.TotalQuantity), timesheetReview.TotalQuantity);
            return Ok(result);
        }
    }
}

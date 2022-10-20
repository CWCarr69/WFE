using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Referential.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReferentialController : ControllerBase
    {
        private readonly IQueryReferential _referentialQuery;

        public ReferentialController(IQueryReferential referentialQuery)
        {
            this._referentialQuery = referentialQuery;
        }

        [HttpGet("TimeoffTypes")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimeoffType>>>> GetTimeoffTypes()
        {
            var timeoffs = _referentialQuery.GetTimeoffTypes();
            return Ok(timeoffs);
        }

        [HttpGet("Departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var departments = await _referentialQuery.GetDepartments();
            return Ok(departments);
        }

        [HttpGet("PayrollPeriods")]
        public async Task<ActionResult<IEnumerable<EmployeeLight>>> GetPayrollPeriods()
        {
            var periods = await _referentialQuery.GetPayrollPeriods();
            return Ok(periods);
        }

        [HttpGet("TimesheetStatuses")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimesheetStatus>>>> GetTimesheetStatuses()
        {
            var statuses = _referentialQuery.GetTimesheetStatuses();
            return Ok(statuses);
        }

        [HttpGet("TimesheetEntryStatuses")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimesheetEntryStatus>>>> GetTimesheetEntryStatuses()
        {
            var statuses = _referentialQuery.GetTimesheetEntryStatuses();
            return Ok(statuses);
        }

        [HttpGet("TimeoffStatuses")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimeoffStatus>>>> GetTimeoffStatuses()
        {
            var statuses = _referentialQuery.GetTimeoffStatuses();
            return Ok(statuses);
        }
    }
}

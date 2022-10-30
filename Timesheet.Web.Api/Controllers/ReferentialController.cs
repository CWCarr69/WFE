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
    public class ReferentialController : BaseController<ReferentialController>
    {
        private readonly IQueryReferential _referentialQuery;

        public ReferentialController(IQueryReferential referentialQuery, ILogger<ReferentialController> logger)
            :base(logger)
        {
            this._referentialQuery = referentialQuery;
        }

        [HttpGet("TimeoffTypes")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimeoffType>>>> GetTimeoffTypes()
        {
            LogInformation($"Listing Timeoff types");

            var timeoffs = _referentialQuery.GetTimeoffTypes();
            return Ok(timeoffs);
        }

        [HttpGet("Departments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            LogInformation($"Listing Departments");

            var departments = await _referentialQuery.GetDepartments();
            return Ok(departments);
        }

        [HttpGet("PayrollPeriods")]
        public async Task<ActionResult<IEnumerable<EmployeeLight>>> GetPayrollPeriods()
        {
            LogInformation($"Listing Payroll periods");
            
            var periods = await _referentialQuery.GetPayrollPeriods();
            return Ok(periods);
        }

        [HttpGet("TimesheetStatuses")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimesheetStatus>>>> GetTimesheetStatuses()
        {
            LogInformation($"Listing Timesheet statuses");
            
            var statuses = _referentialQuery.GetTimesheetStatuses();
            return Ok(statuses);
        }

        [HttpGet("TimesheetEntryStatuses")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimesheetEntryStatus>>>> GetTimesheetEntryStatuses()
        {
            LogInformation($"Listing Timesheet entry statuses");
            
            var statuses = _referentialQuery.GetTimesheetEntryStatuses();
            return Ok(statuses);
        }

        [HttpGet("TimeoffStatuses")]
        public async Task<ActionResult<IEnumerable<EnumReadModel<TimeoffStatus>>>> GetTimeoffStatuses()
        {
            LogInformation($"Listing Timeoff statuses");

            var statuses = _referentialQuery.GetTimeoffStatuses();
            return Ok(statuses);
        }
    }
}

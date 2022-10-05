using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Referential.Queries;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Web.Api.Controllers
{
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
        public async Task<ActionResult<IEnumerable<TimeoffType>>> GetTimeoffTypes()
        {
            var timeoffs = await _referentialQuery.GetTimeoffTypes();
            return Ok(timeoffs);
        }

        [HttpGet("Employees")]
        public async Task<ActionResult<IEnumerable<EmployeeLight>>> GetEmployees()
        {
            var employees = await _referentialQuery.GetEmployees();
            return Ok(employees);
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
        public async Task<ActionResult<IEnumerable<TimesheetStatus>>> GetTimesheetStatuses()
        {
            var statuses = await _referentialQuery.GetTimesheetStatuses();
            return Ok(statuses);
        }
    }
}

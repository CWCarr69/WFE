using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IQueryEmployee _employeeQuery;
        private readonly IDispatcher _dispatcher;

        public EmployeeController(IQueryEmployee employeeQuery, IDispatcher dispatcher)
        {
            _employeeQuery = employeeQuery;
            _dispatcher = dispatcher;
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeProfile>> Get(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeProfile(employeeId);
            return Ok(employee);
        }


        [HttpGet("{employeeId}/Approvers")]
        public async Task<ActionResult<EmployeeApprovers>> GetApprovers(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeApprovers(employeeId);
            return Ok(employee);
        }

        [HttpGet("{employeeId}/Benefits")]
        public async Task<ActionResult<EmployeeApprovers>> GetBenefits(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeBenefits(employeeId);
            return Ok(employee);
        }

        [HttpGet("Team")]
        public async Task<ActionResult<IEnumerable<EmployeeWithTimeStatus>>> GetTeamTimeRecordStatus(bool directReport)
        {
            string managerId = "";
            var employees = await _employeeQuery.GetEmployeesTimeRecordStatus(managerId, directReport);
            return Ok(employees);
        }

        [HttpGet("Timeoff/Pending")]
        public ActionResult<IEnumerable<EmployeeTimeoff>> GetTeamPendingTimeoffs(bool directReport)
        {
            string managerId = "";
            var employees = _employeeQuery.GetEmployeesPendingTimeoffs(managerId, directReport);
            return Ok(employees);
        }

        [HttpGet("Timesheet/Pending")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoff>>> GetTeamPendingTimesheets(bool directReport)
        {
            string managerId = "";
            var employees = await _employeeQuery.GetEmployeesPendingTimeoffs(managerId, directReport);
            return Ok(employees);
        }


    }
}

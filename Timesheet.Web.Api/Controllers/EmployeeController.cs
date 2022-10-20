using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Queries;
using Timesheet.Domain.Employees.Services;
using Timesheet.Domain.ReadModels.Employees;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IQueryEmployee _employeeQuery;
        private readonly IEmployeeBenefitCalculator _benefitsServcies;

        public EmployeeController(IQueryEmployee employeeQuery,
            IEmployeeBenefitCalculator benefitsServcies,
            IDispatcher dispatcher)
        {
            _employeeQuery = employeeQuery;
            this._benefitsServcies = benefitsServcies;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeProfile>>> Get()
        {
            var employee = await _employeeQuery.GetEmployees();
            return Ok(employee);
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
        public async Task<ActionResult<EmployeeBenefits>> GetBenefits(string employeeId)
        {
            var employee = await _employeeQuery.GetEmployeeProfile(employeeId);
            if(employee?.Id is null || employee?.EmploymentDate is null)
            {
                return BadRequest("Employee not found");
            }
            var employeeBenefits = await _benefitsServcies.GetBenefits(employeeId, employee.EmploymentDate.Value);
            return Ok(employeeBenefits);
        }

        [HttpGet("Team")]
        public async Task<ActionResult<IEnumerable<EmployeeWithTimeStatus>>> GetTeamTimeRecordStatus(bool directReport)
        {
            string managerId = null;
            var employees = await _employeeQuery.GetEmployeesTimeRecordStatus(managerId, directReport);
            return Ok(employees);
        }

        [HttpGet("Timeoff/Pending")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoff>>> GetTeamPendingTimeoffs(bool directReport)
        {
            string managerId = null;
            var employees = await _employeeQuery.GetEmployeesPendingTimeoffs(managerId, directReport);
            return Ok(employees);
        }

        [HttpGet("Timesheet/Pending")]
        public async Task<ActionResult<IEnumerable<EmployeeTimeoff>>> GetTeamPendingTimesheets(bool directReport)
        {
            string managerId = null;
            var employees = await _employeeQuery.GetEmployeesPendingTimeoffs(managerId, directReport);
            return Ok(employees);
        }


    }
}

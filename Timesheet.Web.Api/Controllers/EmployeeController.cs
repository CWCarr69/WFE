using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Domain.Models;
using Timesheet.ReadModel.Queries;
using Timesheet.ReadModel.ReadModels;

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

        [HttpGet]
        public ActionResult<EmployeeProfileWithApproversAndBenefit> Get(string employeeId)
        {
            var employee = _employeeQuery.GetEmployeeProfile(employeeId);
            return Ok(employee);
        }

        [HttpGet("Team")]
        public ActionResult<IEnumerable<EmployeeWithTimeStatus>> GetTeamTimeRecordStatus(bool directReport)
        {
            string managerId = "";
            var employees = _employeeQuery.GetEmployeesWithTimeRecordStatus(managerId, directReport);
            return Ok(employees);
        }

        [HttpGet("Timeoff/Pending")]
        public ActionResult<IEnumerable<EmployeeWithPendingTimeoffs>> GetTeamPendingTimeoffs(bool directReport)
        {
            string managerId = "";
            var employees = _employeeQuery.GetEmployeesWithPendingTimeoffs(managerId, directReport);
            return Ok(employees);
        }


    }
}

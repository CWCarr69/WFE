using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Referential.Queries;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels;
using Timesheet.Domain.ReadModels.Employees;
using Timesheet.Domain.ReadModels.Referential;
using Timesheet.Models.Referential;
using PayrollTypes = Timesheet.Domain.ReadModels.Referential.PayrollTypes;

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
        public async Task<ActionResult<IEnumerable<PayrollTypes>>> GetTimeoffTypes()
        {
            LogInformation($"Listing Timeoff types");

            var timeoffs = await _referentialQuery.GetTimeoffTypes();
            return Ok(timeoffs);
        }

        [HttpGet("AllTimeoffTypes")]
        public async Task<ActionResult<IEnumerable<PayrollTypes>>> GetAllTimeoffTypes()
        {
            LogInformation($"Listing All Timeoff types");

            var timeoffs = await _referentialQuery.GetAllTimeoffTypes();
            return Ok(timeoffs);
        }

        [HttpGet("NonRegularTimeoffTypes")]
        public async Task<ActionResult<IEnumerable<PayrollTypes>>> GetNonRegularTimeoffTypes()
        {
            LogInformation($"Listing Non Regular Timeoff types");

            var timeoffs = await _referentialQuery.GetTimeoffTypes(false);
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

        [HttpGet("PayrollCodes")]
        public async Task<ActionResult<IEnumerable<EmployeeLight>>> GetRegularPayrollCodes()
        {
            LogInformation($"Listing regular Payroll codes");

            var periods = await _referentialQuery.GetPayrollCodes();
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

        [HttpGet("TimeoffLabels")]
        public async Task<ActionResult<IEnumerable<string>>> GetTimeoffLabels()
        {
            LogInformation($"Listing Timeoff labels");

            var statuses = await _referentialQuery.GetTimeoffLabels();
            return Ok(statuses);
        }

        [HttpGet("Jobs")]
        public async Task<ActionResult<IEnumerable<SimpleDictionaryItem>>> GetJobs()
        {
            LogInformation($"Listing Jobs");

            var statuses = await _referentialQuery.GetJobs();
            return Ok(statuses);
        }

        [HttpGet("JobTasks")]
        public async Task<ActionResult<IEnumerable<SimpleDictionaryItem>>> GetJobTasks()
        {
            LogInformation($"Listing Job tasks");

            var statuses = await _referentialQuery.GetJobTasks();
            return Ok(statuses);
        }

        [HttpGet("ServiceOrders")]
        public async Task<ActionResult<IEnumerable<SimpleDictionaryItem>>> GetServiceOrders()
        {
            LogInformation($"Listing Service Orders");

            var statuses = await _referentialQuery.GetServiceOrders();
            return Ok(statuses);
        }

        [HttpGet("CustomerNumber")]
        public async Task<ActionResult<IEnumerable<string>>> GetCustomerNumbers()
        {
            LogInformation($"Listing Customer number");

            var customerNumbers = await _referentialQuery.GetCustomerNumbers();
            return Ok(customerNumbers);
        }

        [HttpGet("ProfitCenters")]
        public async Task<ActionResult<IEnumerable<string>>> GetProfitCenters()
        {
            LogInformation($"Listing profit centers");

            var profitCenters = await _referentialQuery.GetProfitCenters();
            return Ok(profitCenters);
        }

        [HttpGet("LaborCodes")]
        public async Task<ActionResult<IEnumerable<string>>> GetLaborCodes()
        {
            LogInformation($"Listing profit centers");

            var profitCenters = await _referentialQuery.GetLaborCodes();
            return Ok(profitCenters);
        }
    }
}

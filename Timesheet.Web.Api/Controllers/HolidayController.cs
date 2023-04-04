using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Holidays.Commands;
using Timesheet.Application.Holidays.Queries;
using Timesheet.Domain.ReadModels.Holidays;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : BaseController<HolidayController>
    {
        private readonly IQueryHoliday _holidayQuery;
        private readonly IDispatcher _dispatcher;

        public HolidayController(IQueryHoliday holidayQuery, IDispatcher dispatcher, ILogger<HolidayController> logger)
            :base(logger)
        {
            _holidayQuery = holidayQuery;
            _dispatcher = dispatcher;
        }

        [HttpGet()]
        [Authorize]
        public async Task<ActionResult<IEnumerable<HolidayDetails>>> Get()
        {
            LogInformation($"Getting Holidays");

            var holidays = await _holidayQuery.GetAllHolidays();
            return Ok(holidays);
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<HolidayDetails?> Get(string id)
        {
            LogInformation($"Getting Holiday ({id}) Details");

            return Ok(_holidayQuery.GetDetails(id));
        } 
        
        [HttpPost]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Post([FromBody] AddHoliday addHoliday, CancellationToken token)
        {
            LogInformation($"Adding Holiday ({addHoliday.Description}) on {addHoliday.Date.ToShortDateString()}");

            await _dispatcher.RunCommand(addHoliday, CurrentUser, token);

            LogInformation($"Holiday Added");
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> UpdateGeneralInformations([FromBody] UpdateHolidayGeneralInformations updateHoliday, CancellationToken token)
        {
            LogInformation($"Updating Holiday ({updateHoliday.Id} - {updateHoliday.Description})");

            await _dispatcher.RunCommand(updateHoliday, CurrentUser, token);

            LogInformation($"Holiday Updated");
            return Ok();
        }

        [HttpPut("/setAsRecurrent")]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> SetAsRecurrent([FromBody] SetHolidayAsRecurrent setHolidayAsRecurrent, CancellationToken token)
        {
            LogInformation($"Set Holiday ({setHolidayAsRecurrent.Id}) as recurrent");

            await _dispatcher.RunCommand(setHolidayAsRecurrent, CurrentUser, token);

            LogInformation($"Holiday set as recurrent");
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Delete([FromBody] DeleteHoliday deleteHoliday, CancellationToken token)
        {
            LogInformation($"Delete Holiday ({deleteHoliday.Id})");

            await _dispatcher.RunCommand(deleteHoliday, CurrentUser, token);
            
            LogInformation($"Holiday deleted");
            return Ok();
        }
    }
}

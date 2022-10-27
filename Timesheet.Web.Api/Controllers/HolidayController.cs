using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Holidays.Commands;
using Timesheet.Application.Holidays.Queries;
using Timesheet.Domain.ReadModels.Holidays;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : BaseController
    {
        private readonly IQueryHoliday _holidayQuery;
        private readonly IDispatcher _dispatcher;

        public HolidayController(IQueryHoliday holidayQuery, IDispatcher dispatcher)
        {
            _holidayQuery = holidayQuery;
            _dispatcher = dispatcher;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<HolidayDetails>>> Get()
        {
            var holidays = await _holidayQuery.GetAllHolidays();
            return Ok(holidays);
        }


        [HttpGet("{id}")]
        public ActionResult<HolidayDetails?> Get(string id)
        {
            return Ok(_holidayQuery.GetDetails(id));
        } 
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddHoliday addHoliday, CancellationToken token)
        {
            await _dispatcher.RunCommand(addHoliday, CurrentUser, token);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGeneralInformations([FromBody] UpdateHolidayGeneralInformations updateHoliday, CancellationToken token)
        {
            await _dispatcher.RunCommand(updateHoliday, CurrentUser, token);
            return Ok();
        }

        [HttpPut("/setAsRecurrent")]
        public async Task<IActionResult> SetAsRecurrent([FromBody] SetHolidayAsRecurrent setHolidayAsRecurrent, CancellationToken token)
        {
            await _dispatcher.RunCommand(setHolidayAsRecurrent, CurrentUser, token);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteHoliday deleteHoliday, CancellationToken token)
        {
            await _dispatcher.RunCommand(deleteHoliday, CurrentUser, token);
            return Ok();
        }
    }
}

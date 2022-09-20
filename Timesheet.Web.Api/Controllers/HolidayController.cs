﻿using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Holidays.Commands;
using Timesheet.ReadModel.Queries;
using Timesheet.ReadModel.ReadModels;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IQueryHoliday _holidayQuery;
        private readonly IDispatcher _dispatcher;

        public HolidayController(IQueryHoliday holidayQuery, IDispatcher dispatcher)
        {
            _holidayQuery = holidayQuery;
            _dispatcher = dispatcher;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<HolidayDetails>> Get()
        {
            return Ok(_holidayQuery.GetAllHolidays());
        }


        [HttpGet("{id}")]
        public ActionResult<HolidayDetails?> Get(string id)
        {
            return Ok(_holidayQuery.GetDetails(id));
        } 
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddHoliday addHoliday, CancellationToken token)
        {
            await _dispatcher.RunCommand(addHoliday, token);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGeneralInformations([FromBody] UpdateHolidayGeneralInformations updateHoliday, CancellationToken token)
        {
            await _dispatcher.RunCommand(updateHoliday, token);
            return Ok();
        }

        [HttpPut("/setAsRecurrent")]
        public async Task<IActionResult> SetAsRecurrent([FromBody] SetHolidayAsRecurrent setHolidayAsRecurrent, CancellationToken token)
        {
            await _dispatcher.RunCommand(setHolidayAsRecurrent,token);
            return Ok();
        }

        [HttpDelete()]
        public async Task<IActionResult> Delete([FromBody] DeleteHoliday deleteHoliday, CancellationToken token)
        {
            await _dispatcher.RunCommand(deleteHoliday, token);
            return Ok();
        }
    }
}
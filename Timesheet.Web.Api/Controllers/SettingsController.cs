using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Holidays.Queries;
using Timesheet.Application.Settings.Commands;
using Timesheet.Domain.Models.Settings;
using Timesheet.Domain.ReadModels.Settings;

namespace Timesheet.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly IQuerySetting _query;
        private readonly IDispatcher _dispatcher;

        public SettingsController(IQuerySetting query, IDispatcher dispatcher)
        {
            _query = query;
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingDetails>>> GetSettings()
        {
            var settings = await _query.GetSettings();
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSetting(UpdateSetting updateSetting, CancellationToken token)
        {
            await _dispatcher.RunCommand(updateSetting, token);
            return Ok();
        }
    }
}

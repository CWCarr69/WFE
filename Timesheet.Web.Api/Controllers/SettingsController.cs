using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Settings.Commands;
using Timesheet.Application.Settings.Queries;
using Timesheet.Domain.ReadModels.Settings;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : BaseController
    {
        private readonly IQuerySetting _query;
        private readonly IDispatcher _dispatcher;

        public SettingsController(IQuerySetting query, IDispatcher dispatcher)
        {
            _query = query;
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingDetailsGrouped>>> GetSettings()
        {
            var settings = await _query.GetSettings();
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSetting(UpdateSetting updateSetting, CancellationToken token)
        {
            await _dispatcher.RunCommand(updateSetting, CurrentUser, token);
            return Ok();
        }
    }
}

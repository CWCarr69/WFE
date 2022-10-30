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
    public class SettingsController : BaseController<SettingsController>
    {
        private readonly IQuerySetting _query;
        private readonly IDispatcher _dispatcher;

        public SettingsController(IQuerySetting query, IDispatcher dispatcher, ILogger<SettingsController> logger)
            :base(logger)
        {
            _query = query;
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SettingDetailsGrouped>>> GetSettings()
        {
            LogInformation($"Visiting Settings");

            var settings = await _query.GetSettings();
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSetting(UpdateSetting updateSetting, CancellationToken token)
        {
            LogInformation($"Updating Settings");

            await _dispatcher.RunCommand(updateSetting, CurrentUser, token);

            LogInformation($"Settings Updated");
            return Ok();
        }
    }
}

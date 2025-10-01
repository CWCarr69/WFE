using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Notifications.Commands;
using Timesheet.Application.Purge.Commands;

namespace Timesheet.Web.Api.Controllers
{ 
  [Authorize(Roles = "ADMINISTRATOR")]
  [Route("api/[controller]")]
  [ApiController]
  public class PurgeController : BaseController<PurgeController>
  {
    private readonly IDispatcher _dispatcher;

    public PurgeController(IDispatcher dispatcher,
            ILogger<PurgeController> logger)
            :base(logger)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Purge audit data older than 2 years.
    /// </summary>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Action result indicating the outcome of the purge operation.</returns>
    [HttpDelete("purge-old")]
    public async Task<IActionResult> PurgeOldData(CancellationToken token)
    {
      var olderThan = new DateTime(DateTime.Now.Year - 2, 1, 1);

      var command = new PurgeOldDataCommand
      {
        OlderThan = olderThan
      };
      await _dispatcher.RunCommand(command, CurrentUser, token);
      return Ok(new { message = $"Timesheet data older than {olderThan:yyyy-MM-dd} purged." });
    }
  }
}

using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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
    /// Purge audit data older than the specified date.
    /// </summary>
    /// <param name="purgeBeforeDate">Optional. The cutoff date for purging data. If not provided, defaults to 2 years ago.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>Action result indicating the outcome of the purge operation.</returns>
    [HttpDelete("purge-old")]
    public async Task<IActionResult> PurgeOldData([FromBody] string? purgeBeforeDate, CancellationToken token)
    {
      // Default to 2 years ago if no date provided
      bool ok = DateTime.TryParse(
          purgeBeforeDate,
          CultureInfo.InvariantCulture,
          DateTimeStyles.RoundtripKind,
          out DateTime olderThan
      );

      if (!ok)
      {
        olderThan = new DateTime(DateTime.Now.Year - 2, 1, 1);
      }
      // Validate date is not in the future
      if (olderThan > DateTime.Now)
      {
        return BadRequest(new { message = "Purge date cannot be in the future." });
      }

      // Validate date is not too recent (optional safety check - adjust threshold as needed)
      var minimumPurgeDate = DateTime.Now.AddMonths(-12);
      if (olderThan > minimumPurgeDate)
      {
        return BadRequest(new { message = $"Purge date must be at least 12 months in the past. Earliest allowed: {minimumPurgeDate:yyyy-MM-dd}" });
      }

      var command = new PurgeOldDataCommand
      {
        OlderThan = olderThan
      };

      try
      {
        await _dispatcher.RunCommand(command, CurrentUser, token);
        
        return Ok(new 
        { 
          message = $"Timesheet data older than {olderThan:yyyy-MM-dd} purged successfully.",
          result = command.Result
        });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Purge operation failed. Please check logs for details." });
      }
    }
  }
}

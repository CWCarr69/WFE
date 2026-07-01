using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Timesheet.Application;
using Timesheet.Application.Employees.Commands;
using Timesheet.Application.Notifications.Commands;
using Timesheet.Application.PayPeriod.Commands;
using Timesheet.Application.Purge.Commands;

namespace Timesheet.Web.Api.Controllers
{
  [Authorize(Roles = "ADMINISTRATOR")]
  [Route("api/[controller]")]
  [ApiController]
  public class PayPeriodsController : BaseController<PayPeriodsController>
  {
    private readonly IDispatcher _dispatcher;

    public PayPeriodsController(IDispatcher dispatcher,
            ILogger<PayPeriodsController> logger)
            : base(logger)
    {
      _dispatcher = dispatcher;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayPeriod([FromBody] string? startFrom, CancellationToken token)
    {

      // Default to 2 years ago if no date provided
      bool ok = DateTime.TryParse(
          startFrom,
          CultureInfo.InvariantCulture,
          DateTimeStyles.RoundtripKind,
          out DateTime startFromDate
      );

      if (!ok)
      {
        startFromDate = new DateTime(DateTime.Now.Year + 1, 1, 1);
      }
      // Validate date is not in the future
      if (startFromDate < DateTime.Now.AddMonths(-6))
      {
        return BadRequest(new { message = "Pay periods start from date cannot be in the past." });
      }

      var command = new PayPeriodCreateCommand
      {
        StartFrom = startFromDate
      };

      try
      {
        await _dispatcher.RunCommand(command, CurrentUser, token);
        LogInformation($"Pay Period Created");
        return Ok();
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An error occurred while creating the pay period." });
      }
    }
  }
}
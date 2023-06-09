using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.EmailSender.Services;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTestController : ControllerBase
    {
        private INotificationService _emailSender;
        private ILogger<EmailTestController> _logger;

        public EmailTestController(INotificationService emailSender, ILogger<EmailTestController> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<bool> Send()
        {
            try
            {
                _logger.LogInformation("START TEST NOTIFICATION");
                _emailSender.SendTestNotifications();
                _logger.LogInformation("END TEST NOTIFICATION");
                return Ok();
            }catch(Exception ex)
            {
                _logger.LogInformation("TEST NOTIFICATION FAILED");
                return Problem(ex.Message);
            }
        }
    }
}

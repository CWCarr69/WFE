using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timesheet.Application;
using Timesheet.Application.Notifications.Commands;
using Timesheet.Application.Notifications.Queries;
using Timesheet.Application.Notifications.Services;
using Timesheet.Web.Api.ViewModels;

namespace Timesheet.Web.Api.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController<NotificationController>
    {
        private readonly IQueryNotification _query;
        private readonly IDispatcher _dispatcher;
        private readonly INotificationPopulationServices _populationServices;

        public NotificationController(
            IQueryNotification query,
            IDispatcher dispatcher,
            INotificationPopulationServices populationServices,
            ILogger<NotificationController> logger)
            :base(logger)
        {
            _query = query;
            _dispatcher = dispatcher;
            this._populationServices = populationServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            LogInformation($"Visiting notifications configuraton");

            var notifications = await _query.GetNotifications();
            var data = notifications.Select(n =>
            {
                var populations = _populationServices.Deconstruct(n.Population);
                return new NotificationDto
                {
                    Id = n.Id,
                    Group = n.Group,
                    Action = n.Action,
                    Populations = NotificationPopulationServices.AllPopulation
                        .Select(p => new NotificationPopulationDto
                        {
                            Name = p.ToString(),
                            Value = (int)p,
                            IsActive = populations.Contains(p)
                        })
                };
            });

            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNotification([FromBody] NotificationUpdateModel request, CancellationToken token)
        {
            LogInformation($"Modifying notifications");

            var command = new UpdateNotification
            {
                Id = request.Id,
                Population = _populationServices.Construct(request.Populations)
            };

            await _dispatcher.RunCommand(command, CurrentUser, token);

            LogInformation($"Notifications modified");
            return Ok();
        }
    }
}

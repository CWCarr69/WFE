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
    public class NotificationController : BaseController
    {
        private readonly IQueryNotification _query;
        private readonly IDispatcher _dispatcher;
        private readonly INotificationPopulationServices _populationServices;

        public NotificationController(
            IQueryNotification query,
            IDispatcher dispatcher,
            INotificationPopulationServices populationServices)
        {
            _query = query;
            _dispatcher = dispatcher;
            this._populationServices = populationServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            var notifications = await _query.GetNotifications();
            var data = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Populations = _populationServices.Deconstruct(n.Population)
                .Select(p => new NotificationPopulationDto
                {
                    Name = p.ToString(),
                    Value = (int)p
                })
            });

            return Ok(data);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNotification([FromBody] NotificationUpdateModel request, CancellationToken token)
        {
            var command = new UpdateNotification
            {
                Id = request.Id,
                Population = _populationServices.Construct(request.Populations)
            };

            await _dispatcher.RunCommand(command, CurrentUser, token);
            return Ok();
        }
    }
}

using Serilog.Context;
using System.Security.Claims;

namespace Timesheet.Web.Api.Middleware
{
    public class LogUserNameMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var username = context.User.Identity.Name;
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            LogContext.PushProperty("Username", $"{username} ({userId})");

            await next(context);
        }
    }
}

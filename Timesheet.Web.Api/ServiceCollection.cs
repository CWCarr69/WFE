using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Timesheet.Domain.Exceptions;

namespace Timesheet.Web.Api
{
    public static class ServiceCollection
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    if(exceptionHandlerPathFeature != null)
                    {
                        Log.Error($"{exceptionHandlerPathFeature.Error}");

                        var exception = exceptionHandlerPathFeature.Error;

                        context.Response.StatusCode = exceptionHandlerPathFeature.Error switch
                        {
                            DomainException domainException => domainException.Code,
                            _ => StatusCodes.Status500InternalServerError
                        };

                        if (exception is DomainException ex)
                        {
                            await context.Response.WriteAsJsonAsync(ex.ToProblem());
                        }
                        else
                        {
                            await context.Response.WriteAsJsonAsync(
                                new Problem("Unknown error, we are investigating.", StatusCodes.Status500InternalServerError, "Internal"));
                        }
                    }
                });
            });
        }
    }
}

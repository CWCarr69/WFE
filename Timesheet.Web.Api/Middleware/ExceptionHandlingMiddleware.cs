using Microsoft.AspNetCore.Mvc;
using Timesheet.Domain.Exceptions;

namespace Timesheet.Web.Api.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;
           
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                DomainException domainException => domainException.Code,
                _ => StatusCodes.Status500InternalServerError
            };

            if(exception is DomainException ex)
            {
                await context.Response.WriteAsJsonAsync(ex.ToProblem());
            }
            else
            {
                await context.Response.WriteAsJsonAsync(
                    new Problem("Unknown error, we are investigating.", StatusCodes.Status500InternalServerError, "Internal"));
            }

        }
    }
}

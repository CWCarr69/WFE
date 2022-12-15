using Microsoft.Extensions.Logging;

namespace Timesheet.Web.Api.ServiceWorker
{
    public class TimesheetWebApiService : BackgroundService
    {
        private readonly ILogger<TimesheetWebApiService> _logger;

        public TimesheetWebApiService(ILogger<TimesheetWebApiService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timesheet Web API is starting.");

            stoppingToken.Register(() => _logger.LogInformation("Timesheet Web API is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("Timesheet Web API has stopped.");
        }
    }
}
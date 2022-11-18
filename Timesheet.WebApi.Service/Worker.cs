namespace Timesheet.WebApi.Service
{
    public class WebAPI : BackgroundService
    {
        private readonly ILogger<WebAPI> _logger;

        public WebAPI(ILogger<WebAPI> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
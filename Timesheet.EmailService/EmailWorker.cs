using Timesheet.EmailSender.Services;

namespace Timesheet.EmailService
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        private readonly INotificationService _notificationService;
        private const long DEFAULT_EMAIL_PROCESSING_FREQUENCY = 30000;

        public EmailWorker(ILogger<EmailWorker> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Email worker running at: {time}", DateTimeOffset.Now);
                _notificationService.SendNotifications();

                await Delay(DEFAULT_EMAIL_PROCESSING_FREQUENCY, stoppingToken);
            }
        }

        static async Task Delay(long delay, CancellationToken stoppingToken)
        {
            while (delay > 0)
            {
                var currentDelay = delay > int.MaxValue ? int.MaxValue : (int)delay;
                await Task.Delay(currentDelay, stoppingToken);
                delay -= currentDelay;
            }
        }
    }
}
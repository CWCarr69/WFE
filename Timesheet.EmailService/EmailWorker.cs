using Timesheet.EmailSender.Services;

namespace Timesheet.EmailService
{
    public class EmailWorker : BackgroundService
    {
        private readonly ILogger<EmailWorker> _logger;
        private readonly INotificationService _notificationService;
        private const int DEFAULT_EMAIL_PROCESSING_FREQUENCY = 300000;

        private readonly object lockObject = new object();
        private bool isRunning = false;

        public EmailWorker(ILogger<EmailWorker> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!isRunning)
                {
                    _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
                    isRunning = true;
                    await Task.Run(WorkerMethod);
                }

                await Task.Delay(DEFAULT_EMAIL_PROCESSING_FREQUENCY);
            }
        }

        private void WorkerMethod()
        {
            try
            {

                _logger.LogInformation("Email worker running at: {time}", DateTimeOffset.Now);
                _notificationService.SendNotifications();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email worker encounter an error: {ex}");
            }
            finally
            {
                lock (lockObject)
                {
                    isRunning = false;
                }
            }
        }
    }
}
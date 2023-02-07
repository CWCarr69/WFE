using Timesheet.FDPDataIntegrator.Services;
using Timesheet.FDPIntegratorService.Service;

namespace Timesheet.FDPIntegratorService
{
    public class FDPIntegratorWorker : BackgroundService
    {
        private readonly IFDPIntegratorProcess _fdpIntegrator;
        private readonly ILogger<FDPIntegratorWorker> _logger;
        private readonly long _uploadFrequency;
        private const long DEFAULT_UPLOAD_FREQUENCY = 3600000;

        public FDPIntegratorWorker(IFDPIntegratorProcess fdpIntegrator, ISettingRepository settingRepository, ILogger<FDPIntegratorWorker> logger)
        {
            _fdpIntegrator = fdpIntegrator;
            _logger = logger;

            var settings = settingRepository.GetFDPParameters();
            long.TryParse(settings.FDP_UploadFrequency, out _uploadFrequency);
            _uploadFrequency = _uploadFrequency == 0 ? DEFAULT_UPLOAD_FREQUENCY : _uploadFrequency * 3600000;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("FDPIntegrator Worker running at: {time}", DateTimeOffset.Now);
                _fdpIntegrator.ProcessEmployees().Wait();
                _fdpIntegrator.ProcessPayrolls().Wait();
                
                await Delay(_uploadFrequency, stoppingToken);
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
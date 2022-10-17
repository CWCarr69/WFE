using Timesheet.FDPIntegratorService.Service;

namespace Timesheet.FDPIntegratorService
{
    public class FDPIntegratorWorker : BackgroundService
    {
        private readonly IFDPIntegratorProcess _fdpIntegrator;
        private readonly ILogger<FDPIntegratorWorker> _logger;

        public FDPIntegratorWorker(IFDPIntegratorProcess fdpIntegrator, ILogger<FDPIntegratorWorker> logger)
        {
            _fdpIntegrator = fdpIntegrator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("FDPIntegrator Worker running at: {time}", DateTimeOffset.Now);
                _fdpIntegrator.ProcessEmployees().Wait();
                _fdpIntegrator.ProcessPayrolls().Wait();
                
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
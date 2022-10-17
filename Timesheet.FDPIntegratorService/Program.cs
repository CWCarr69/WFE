using Timesheet.FDPIntegratorService.Service;
using Timesheet.FDPDataIntegrator;

namespace Timesheet.FDPIntegratorService
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureLogging(logging =>
               {
                   logging.ClearProviders();
                   logging.AddConsole();
                   logging.AddEventLog();
               })
               // Essential to run this as a window service
               .UseWindowsService()
               .ConfigureServices(configureServices);

        private static void configureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddLogging();
            services.AddServices(context.Configuration.GetConnectionString("Timesheet"));
            services.AddSingleton<IFDPIntegratorProcess, FDPIntegratorProcess>();
            services.AddHostedService<FDPIntegratorWorker>();
        }
    }
}
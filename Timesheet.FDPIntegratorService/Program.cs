using Timesheet.FDPIntegratorService.Service;
using Timesheet.Infrastructure.Dapper;
using Timesheet.FDPDataIntegrator;
using Serilog;

namespace Timesheet.FDPIntegratorService
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) {
           return Host.CreateDefaultBuilder(args)
               .ConfigureLogging(logging =>
               {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddEventLog();
                })
               // Essential to run this as a window service
               .UseWindowsService()
               .ConfigureServices(Program.ConfigureServices)
               .UseSerilog(((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)

                    .Enrich.FromLogContext()
                ));
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddLogging()
            .AddServices(context.Configuration.GetConnectionString("Timesheet"))
            .AddSingletonDatabaseQueryService()
            .AddSingleton<IFDPIntegratorProcess, FDPIntegratorProcess>()
            .AddHostedService<FDPIntegratorWorker>();
        }
    }
}
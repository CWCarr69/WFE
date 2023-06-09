using Serilog;
using Timesheet.EmailSender;

namespace Timesheet.EmailService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddEventLog();
                })
                .UseWindowsService()
                .ConfigureServices(Program.ConfigureServices)
                .UseSerilog(((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                    .Enrich.FromLogContext()
                ))
                .Build();

            host.Run();
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var connectionString = context.Configuration.GetConnectionString("Notification");
            var webAppUri = context.Configuration["WebAppUri"];
            var templatesBasePath = context.Configuration["templatesBasePath"];

            services.AddLogging()
            .AddEmailServices(sp => new NotificationSqlConnection( connectionString ), webAppUri, templatesBasePath)
            .AddHostedService<EmailWorker>();
        }
    }
}
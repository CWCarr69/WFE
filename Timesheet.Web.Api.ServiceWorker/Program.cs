namespace Timesheet.Web.Api.ServiceWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TimesheetWebApiService>();
                })
                .Build();

            host.Run();
        }
    }
}
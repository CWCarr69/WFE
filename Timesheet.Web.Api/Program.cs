using Timesheet.Application;
using Timesheet.Infrastructure.Persistence;
using Timesheet.ReadModel;

namespace Timesheet.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddTimesheetContext(builder.Configuration.GetConnectionString("Timesheet"));
            builder.Services.AddTimesheedReadModelDatabase(builder.Configuration.GetConnectionString("Timesheet"));

            builder.Services.AddWorkflowService();
            builder.Services.RegisterEventDispatcher();
            builder.Services.RegisterEventHandlers();
            builder.Services.RegisterCommandHandlers();
            builder.Services.AddOtherServices();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
            });

            var app = builder.Build();

            //Register Handlers
            ConfigureHandlers(app);

            // Define global exception handling
            app.ConfigureExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureHandlers(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var handlersConfiguration = scope.ServiceProvider.GetRequiredService(typeof(HandlersConfiguration)) as HandlersConfiguration;
                if (handlersConfiguration == null)
                {
                    throw new Exception("Cannot configure event handlers. Program cannot start.");
                }
                handlersConfiguration.RegisterHandlers();
            }
        }
    }
}
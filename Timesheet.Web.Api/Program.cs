using Microsoft.AspNetCore.Authentication.JwtBearer;
using Timesheet.Application;
using Timesheet.Infrastructure.Persistence;
using Timesheet.ReadModel;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Timesheet.Infrastructure.Authentication;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Serilog;
using Timesheet.Web.Api.Middleware;

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
            builder.Services.AddAuthenticationServices();
            builder.Services.AddTimesheetExportServices(builder.Configuration.GetSection("TimesheetExport:Destination").Value);
            builder.Services.AddOtherServices();


            //Logging
            builder.Services.AddTransient<LogUserNameMiddleware>();

            Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
            builder.Host.UseSerilog(((ctx, lc) => lc
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
            ));

            //Controllers
            builder.Services.AddControllers();
            builder.Services.AddCors();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
            options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Authentication using Bearer scheme",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            }
            );

            //Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                            builder.Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            var app = builder.Build();

            //Register Handlers
            ConfigureHandlers(app);

            //Initialize DB
            InitializeDatabase(app);

            //Use serilog
            app.UseSerilogRequestLogging();

            // Define global exception handling
            app.ConfigureExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<LogUserNameMiddleware>();

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

        private static void InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<TimesheetDbContext>();
                    DbInitializer.Initialize(context);
                }catch(Exception ex){
                    throw new Exception($"Cannot initialize Database. Program cannot start. {ex.Message}");
                }
            };
        }
    }
}
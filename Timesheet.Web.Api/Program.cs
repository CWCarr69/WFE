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
using Timesheet.Web.Api.ServiceWorker;
using Hangfire;
using Timesheet.Benefits;

namespace Timesheet.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webApplicationOptions = new WebApplicationOptions() { ContentRootPath = AppContext.BaseDirectory, Args = args, ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName };
            var builder = WebApplication.CreateBuilder(webApplicationOptions);
            builder.Host.UseWindowsService();

            // Add services to the container.
            builder.Services.AddTimesheetContext(builder.Configuration.GetConnectionString("Timesheet"))
            .AddTimesheedReadModelDatabase(builder.Configuration.GetConnectionString("Timesheet"))

            .AddWorkflowService()
            .RegisterEventDispatcher()
            .RegisterEventHandlers()
            .RegisterCommandHandlers()
            .AddAuthenticationServices()
            .AddTimesheetExportServices(builder.Configuration.GetSection("TimesheetExport:Destination").Value)
            .AddOtherApplicationServices();

            builder.Services.AddBenefitsServices();

            //Hangfire
            builder.Services.AddHangfire(config => config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration.GetConnectionString("Timesheet")));
            builder.Services.AddHangfireServer();

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

            //HostedService
            builder.Services.AddHostedService<TimesheetWebApiService>();

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

            app.UseCors(p => p
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithOrigins("https://*.wilsonfire.net", "https://localhost")
            .AllowAnyHeader().AllowAnyMethod()
            .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<LogUserNameMiddleware>();

            app.Use(async (context, next) =>
            {
                var url = context.Request.Path.Value;

                // Rewrite to index
                if (!url.Contains("/api") && !url.Contains("/static"))
                {
                    // rewrite and continue processing
                    context.Request.Path = "/index.html";
                }

                await next();
            });

            app.UseStaticFiles();

            app.MapControllers();

            app.UseHangfireDashboard();
            app.MapHangfireDashboard();

            RecurringJob.AddOrUpdate<IEmployeeBenefitsService>(x => x.UpdateEmployeeBenefits(), builder.Configuration.GetSection("AppSettings:BenefitsCron").Value);

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
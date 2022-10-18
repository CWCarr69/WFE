using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Employees.CommandHandlers;
using Timesheet.Application.Holidays.CommandHandlers;
using Timesheet.Application.Notifications;
using Timesheet.Application.Settings.CommandHandlers;
using Timesheet.Application.Shared;
using Timesheet.Application.Timesheets.EventHandlers;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Employees.Services;

namespace Timesheet.Application
{
    public static class ServiceCollection
    {
        public static void AddWorkflowService(this IServiceCollection services)
        {
            services.AddScoped<IWorkflowRegistry, WorkflowRegistry>();
            services.AddScoped<IWorkflowService, WorkflowService>();
        }

        public static void RegisterEventDispatcher(this IServiceCollection services)
        {
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddSingleton<HandlersConfiguration, HandlersConfiguration>();
        }

        public static void RegisterEventHandlers(this IServiceCollection services)
        {
            services.AddScoped<HolidayAddedHandler, HolidayAddedHandler>();
            services.AddScoped<HolidayDeletedHandler, HolidayDeletedHandler>();
            services.AddScoped<HolidayGeneralInformationsUpdatedHandler, HolidayGeneralInformationsUpdatedHandler>();

            services.AddScoped<TimeoffWorkflowEventHandler, TimeoffWorkflowEventHandler>();
            services.AddScoped<IAuditHandler, AuditHandler>();
        }

        public static void RegisterCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<AddHolidayCommandHandler, AddHolidayCommandHandler>();
            services.AddScoped<DeleteHolidayCommandHandler, DeleteHolidayCommandHandler>();
            services.AddScoped<SetHolidayAsRecurrentCommandHandler, SetHolidayAsRecurrentCommandHandler>();
            services.AddScoped<UpdateHolidayGeneralInformationsCommandHandler, UpdateHolidayGeneralInformationsCommandHandler>();

            services.AddScoped<ModifyApproverCommandHandler, ModifyApproverCommandHandler>();

            services.AddScoped<CreateTimeoffCommandHandler, CreateTimeoffCommandHandler>();
            services.AddScoped<SubmitTimeoffCommandHandler, SubmitTimeoffCommandHandler>();
            services.AddScoped<DeleteTimeoffCommandHandler, DeleteTimeoffCommandHandler>();
            services.AddScoped<ApproveTimeoffCommandHandler, ApproveTimeoffCommandHandler>();
            services.AddScoped<RejectTimeoffCommandHandler, RejectTimeoffCommandHandler>();
            services.AddScoped<AddEntryToTimeoffCommandHandler, AddEntryToTimeoffCommandHandler>();
            services.AddScoped<UpdateTimeoffEntryCommandHandler, UpdateTimeoffEntryCommandHandler>();
            services.AddScoped<DeleteTimeoffEntryCommandHandler, DeleteTimeoffEntryCommandHandler>();
            
            services.AddScoped<UpdateSettingCommandHandler, UpdateSettingCommandHandler>();
        }

        public static void AddOtherServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeBenefitCalculator, EmployeeBenefitCalculator>();
        }

    }
}

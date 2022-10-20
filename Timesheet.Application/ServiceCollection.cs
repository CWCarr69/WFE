using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Employees.CommandHandlers;
using Timesheet.Application.Holidays.CommandHandlers;
using Timesheet.Application.Notifications.EventHandlers;
using Timesheet.Application.Notifications.Services;
using Timesheet.Application.Settings.CommandHandlers;
using Timesheet.Application.Shared;
using Timesheet.Application.Timesheets.CommandHandlers;
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
            services.AddScoped<TimesheetHandleHolidayAdded, TimesheetHandleHolidayAdded>();
            services.AddScoped<TimesheetHandleHolidayDeleted, TimesheetHandleHolidayDeleted>();
            services.AddScoped<TimehsheetHandleHolidayGeneralInformationsUpdated, TimehsheetHandleHolidayGeneralInformationsUpdated>();

            services.AddScoped<TimeoffStateChangedEventHandler, TimeoffStateChangedEventHandler>();
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

            services.AddScoped<SubmitTimesheetCommandHandler, SubmitTimesheetCommandHandler>();
            services.AddScoped<ApproveTimesheetCommandHandler, ApproveTimesheetCommandHandler>();
            services.AddScoped<RejectTimesheetCommandHandler, RejectTimesheetCommandHandler>();
            services.AddScoped<FinalizeTimesheetCommandHandler, FinalizeTimesheetCommandHandler>();

            services.AddScoped<UpdateSettingCommandHandler, UpdateSettingCommandHandler>();

            services.AddScoped<UpdateNotificationCommandHandler, UpdateNotificationCommandHandler>();
        }

        public static void AddOtherServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeBenefitCalculator, EmployeeBenefitCalculator>();
            services.AddScoped<INotificationPopulationServices, NotificationPopulationServices>();
        }
    }
}

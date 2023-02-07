using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Employees.CommandHandlers;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Holidays.CommandHandlers;
using Timesheet.Application.Notifications.CommandHandlers;
using Timesheet.Application.Notifications.EventHandlers;
using Timesheet.Application.Notifications.Services;
using Timesheet.Application.Settings.CommandHandlers;
using Timesheet.Application.Shared;
using Timesheet.Application.Timesheets.CommandHandlers;
using Timesheet.Application.Timesheets.EventHandlers;
using Timesheet.Application.Timesheets.EventHandlers.Export;
using Timesheet.Application.Timesheets.EventHandlers.Holidays;
using Timesheet.Application.Timesheets.Services;
using Timesheet.Application.Timesheets.Services.Export;
using Timesheet.Application.Workflow;
using Timesheet.Domain.Employees.Services;
using Timesheet.Domain.ReadModels.Timesheets;

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
            services.AddScoped<TimesheetStateChangedEventHandler, TimesheetStateChangedEventHandler>();
            
            services.AddScoped<TimesheetHandleTimeoffApproved, TimesheetHandleTimeoffApproved>();
            services.AddScoped<TimehsheetHandleTimesheetFinalized, TimehsheetHandleTimesheetFinalized>();

            services.AddScoped<IAuditHandler, AuditHandler>();
        }

        public static void RegisterCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<AddHolidayCommandHandler, AddHolidayCommandHandler>();
            services.AddScoped<DeleteHolidayCommandHandler, DeleteHolidayCommandHandler>();
            services.AddScoped<SetHolidayAsRecurrentCommandHandler, SetHolidayAsRecurrentCommandHandler>();
            services.AddScoped<UpdateHolidayGeneralInformationsCommandHandler, UpdateHolidayGeneralInformationsCommandHandler>();

            services.AddScoped<ModifyApproverCommandHandler, ModifyApproverCommandHandler>();
            services.AddScoped<ModifyEmployeeBenefitsCommandHandler, ModifyEmployeeBenefitsCommandHandler>();

            services.AddScoped<CreateTimeoffCommandHandler, CreateTimeoffCommandHandler>();
            services.AddScoped<SubmitTimeoffCommandHandler, SubmitTimeoffCommandHandler>();
            services.AddScoped<DeleteTimeoffCommandHandler, DeleteTimeoffCommandHandler>();
            services.AddScoped<ApproveTimeoffCommandHandler, ApproveTimeoffCommandHandler>();
            services.AddScoped<RejectTimeoffCommandHandler, RejectTimeoffCommandHandler>();
            services.AddScoped<AddEntryToTimeoffCommandHandler, AddEntryToTimeoffCommandHandler>();
            services.AddScoped<UpdateTimeoffEntryCommandHandler, UpdateTimeoffEntryCommandHandler>();
            services.AddScoped<DeleteTimeoffEntryCommandHandler, DeleteTimeoffEntryCommandHandler>();
            services.AddScoped<UpdateTimeoffCommentCommandHandler, UpdateTimeoffCommentCommandHandler>();

            services.AddScoped<AddTimesheetEntryCommandHandler, AddTimesheetEntryCommandHandler>();
            services.AddScoped<DeleteTimesheetEntryCommandHandler, DeleteTimesheetEntryCommandHandler>();
            services.AddScoped<SubmitTimesheetCommandHandler, SubmitTimesheetCommandHandler>();
            services.AddScoped<SubmitTimesheetCommandHandler, SubmitTimesheetCommandHandler>();
            services.AddScoped<ApproveTimesheetCommandHandler, ApproveTimesheetCommandHandler>();
            services.AddScoped<RejectTimesheetCommandHandler, RejectTimesheetCommandHandler>();
            services.AddScoped<FinalizeTimesheetCommandHandler, FinalizeTimesheetCommandHandler>();
            services.AddScoped<UpdateTimesheetCommentCommandHandler, UpdateTimesheetCommentCommandHandler>();

            services.AddScoped<AddTimesheetExceptionCommandHandler, AddTimesheetExceptionCommandHandler>();



            services.AddScoped<UpdateSettingCommandHandler, UpdateSettingCommandHandler>();

            services.AddScoped<UpdateNotificationCommandHandler, UpdateNotificationCommandHandler>();
        }

        public static void AddTimesheetExportServices(this IServiceCollection services, string destination)
        {
            services.AddScoped<IExportTimesheetService, ExportTimesheetService>();
            services.AddScoped<ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel>, TimesheetToCSVModelAdapter>();
            services.AddScoped<ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel>, ExternalTimesheetToCSVModelAdapter>();
            services.AddScoped<ITimesheetCSVWriter, TimesheetCSVWriter>();
            services.AddScoped<ITimesheetCSVFormatter, TimesheetCSVFormatter>();

            services.AddScoped<IExportTimesheetDestination>(sp => new ExportTimesheetDestination(destination));
        }

        public static void AddOtherApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeBenefitCalculator, EmployeeBenefitCalculator>();
            services.AddScoped<INotificationPopulationServices, NotificationPopulationServices>();
            services.AddScoped<IEmployeeHabilitation, EmployeeHabilitation>();
            services.AddScoped<ITimesheetPeriodService, TimesheetPeriodService>();
        }
    }
}

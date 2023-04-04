using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
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
        public static IServiceCollection AddWorkflowService(this IServiceCollection services)
        {
            return services.AddScoped<IWorkflowRegistry, WorkflowRegistry>()
            .AddScoped<IWorkflowService, WorkflowService>();
        }

        public static IServiceCollection RegisterEventDispatcher(this IServiceCollection services)
        {
            return services.AddScoped<IDispatcher, Dispatcher>()
                .AddSingleton<HandlersConfiguration, HandlersConfiguration>();
        }

        public static IServiceCollection RegisterEventHandlers(this IServiceCollection services)
        {
            return services.AddScoped<TimesheetHandleHolidayAdded, TimesheetHandleHolidayAdded>()
            .AddScoped<TimesheetHandleHolidayDeleted, TimesheetHandleHolidayDeleted>()
            .AddScoped<TimehsheetHandleHolidayGeneralInformationsUpdated, TimehsheetHandleHolidayGeneralInformationsUpdated>()

            .AddScoped<TimeoffStateChangedEventHandler, TimeoffStateChangedEventHandler>()
            .AddScoped<TimesheetStateChangedEventHandler, TimesheetStateChangedEventHandler>()
            
            .AddScoped<TimesheetHandleTimeoffApproved, TimesheetHandleTimeoffApproved>()
            .AddScoped<TimehsheetHandleTimesheetFinalized, TimehsheetHandleTimesheetFinalized>()

            .AddScoped<IAuditHandler, AuditHandler>();
        }

        public static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
        {
            return services.AddScoped<AddHolidayCommandHandler, AddHolidayCommandHandler>()
            .AddScoped<DeleteHolidayCommandHandler, DeleteHolidayCommandHandler>()
            .AddScoped<SetHolidayAsRecurrentCommandHandler, SetHolidayAsRecurrentCommandHandler>()
            .AddScoped<UpdateHolidayGeneralInformationsCommandHandler, UpdateHolidayGeneralInformationsCommandHandler>()
            
            .AddScoped<ModifyApproverCommandHandler, ModifyApproverCommandHandler>()
            .AddScoped<ModifyEmployeeBenefitsCommandHandler, ModifyEmployeeBenefitsCommandHandler>()
            
            .AddScoped<CreateTimeoffCommandHandler, CreateTimeoffCommandHandler>()
            .AddScoped<SubmitTimeoffCommandHandler, SubmitTimeoffCommandHandler>()
            .AddScoped<DeleteTimeoffCommandHandler, DeleteTimeoffCommandHandler>()
            .AddScoped<ApproveTimeoffCommandHandler, ApproveTimeoffCommandHandler>()
            .AddScoped<RejectTimeoffCommandHandler, RejectTimeoffCommandHandler>()
            .AddScoped<AddEntryToTimeoffCommandHandler, AddEntryToTimeoffCommandHandler>()
            .AddScoped<UpdateTimeoffEntryCommandHandler, UpdateTimeoffEntryCommandHandler>()
            .AddScoped<DeleteTimeoffEntryCommandHandler, DeleteTimeoffEntryCommandHandler>()
            .AddScoped<UpdateTimeoffCommentCommandHandler, UpdateTimeoffCommentCommandHandler>()
            
            .AddScoped<AddTimesheetEntryCommandHandler, AddTimesheetEntryCommandHandler>()
            .AddScoped<DeleteTimesheetEntryCommandHandler, DeleteTimesheetEntryCommandHandler>()
            .AddScoped<SubmitTimesheetCommandHandler, SubmitTimesheetCommandHandler>()
            .AddScoped<SubmitTimesheetCommandHandler, SubmitTimesheetCommandHandler>()
            .AddScoped<ApproveTimesheetCommandHandler, ApproveTimesheetCommandHandler>()
            .AddScoped<RejectTimesheetCommandHandler, RejectTimesheetCommandHandler>()
            .AddScoped<FinalizeTimesheetCommandHandler, FinalizeTimesheetCommandHandler>()
            .AddScoped<UpdateTimesheetCommentCommandHandler, UpdateTimesheetCommentCommandHandler>()
            
            .AddScoped<AddTimesheetExceptionCommandHandler, AddTimesheetExceptionCommandHandler>()

            .AddScoped<UpdateSettingCommandHandler, UpdateSettingCommandHandler>()
            
            .AddScoped<UpdateNotificationCommandHandler, UpdateNotificationCommandHandler>();
        }

        public static IServiceCollection AddTimesheetExportServices(this IServiceCollection services, string destination)
        {
            return services.AddScoped<IExportTimesheetService, ExportTimesheetService>()
            .AddScoped<ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel>, TimesheetToCSVModelAdapter>()
            .AddScoped<ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel>, ExternalTimesheetToCSVModelAdapter>()
            .AddScoped<ITimesheetCSVWriter, TimesheetCSVWriter>()
            .AddScoped<ITimesheetCSVFormatter, TimesheetCSVFormatter>()

            .AddScoped<IExportTimesheetDestination>(sp => new ExportTimesheetDestination(destination));
        }

        public static IServiceCollection AddOtherApplicationServices(this IServiceCollection services)
        {
            return services.AddScoped<IEmployeeBenefitCalculator, EmployeeBenefitCalculator>()
            .AddScoped<INotificationPopulationServices, NotificationPopulationServices>()
            .AddScoped<IEmployeeHabilitation, EmployeeHabilitation>()
            .AddScoped<ITimesheetPeriodService, TimesheetPeriodService>();
        }
    }
}

using Timesheet.Application.Timesheets.Services.Export;
using Timesheet.Domain.DomainEvents;

namespace Timesheet.Application.Timesheets.EventHandlers.Export
{
    internal sealed class TimehsheetHandleTimesheetFinalized : IEventHandler<TimesheetFinalized>
    {
        private readonly IExportTimesheetService _exportTimesheet;

        public TimehsheetHandleTimesheetFinalized(IExportTimesheetService exportTimesheet)
        {
            this._exportTimesheet = exportTimesheet;
        }

        public async Task Handle(TimesheetFinalized @event)
        {
            if(@event is null || @event.payrollPeriod is null)
            {
                return;
            }

            await _exportTimesheet.ExportToExternal(@event.payrollPeriod);
        }
    }
}

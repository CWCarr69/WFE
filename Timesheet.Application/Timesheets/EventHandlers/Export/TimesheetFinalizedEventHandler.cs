using Timesheet.Application.Shared;
using Timesheet.Application.Timesheets.Services.Export;
using Timesheet.Domain.DomainEvents;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Timesheets.EventHandlers.Export
{
    internal sealed class TimehsheetHandleTimesheetFinalized : BaseEventHandler<TimesheetFinalized>
    {
        private readonly IExportTimesheetService _exportTimesheet;

        public TimehsheetHandleTimesheetFinalized(IExportTimesheetService exportTimesheet,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this._exportTimesheet = exportTimesheet;
        }

        public override async Task HandleEvent(TimesheetFinalized @event)
        {
            if(@event is null || @event.payrollPeriod is null)
            {
                return;
            }

            await _exportTimesheet.ExportAdaptedReviewToExternal(@event.payrollPeriod);
        }
    }
}

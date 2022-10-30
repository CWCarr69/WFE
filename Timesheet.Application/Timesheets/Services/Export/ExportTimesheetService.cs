using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Timesheets;

namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class ExportTimesheetService : IExportTimesheetService
    {
        private readonly IQueryTimesheet _timesheets;
        private readonly ITimesheetToCSVModelAdapter _adapter;
        private readonly ITimesheetCSVFormatter _formatter;
        private readonly ITimesheetCSVWriter _csvWriter;
        private readonly string _externalDestinationBasePath;

        private readonly string _paylocityFileName = "Paylocity_Timesheet_";

        public ExportTimesheetService(IQueryTimesheet timesheets,
            ITimesheetToCSVModelAdapter adapter,
            ITimesheetCSVFormatter formatter,
            ITimesheetCSVWriter csvWriter,
            IExportTimesheetDestination exportDestination)
        {
            _externalDestinationBasePath = exportDestination.GetPath();

            if (_externalDestinationBasePath is null)
            {
                throw new ArgumentNullException(nameof(_externalDestinationBasePath));
            }

            _timesheets = timesheets;
            _adapter = adapter;
            _formatter = formatter;
            _csvWriter = csvWriter;
        }

        public async Task ExportToExternal(string payrollPeriod)
        {
            string csv = await GetTimesheetAsCsv(payrollPeriod, true);
            string outpuPath = Path.Combine(_externalDestinationBasePath, $"{_paylocityFileName}{payrollPeriod}");
            _csvWriter.SetPath(outpuPath);
            await _csvWriter.Write(csv);
        }

        public async Task<string> ExportToWeb(string payrollPeriod) 
            => await GetTimesheetAsCsv(payrollPeriod, false);

        private async Task<string> GetTimesheetAsCsv(string payrollPeriod, bool checkFinalizedState)
        {
            if (payrollPeriod is null)
            {
                return string.Empty;
            }

            var timesheetCSVModel = await PrepareExport(payrollPeriod, checkFinalizedState);
            var csv = _formatter.Format(timesheetCSVModel);
            return csv;
        }

        private async Task<TimesheetCSVModel> PrepareExport(string payrollPeriod, bool checkFinalizedState)
        {
            var timesheet = await _timesheets.GetAllEmployeeTimesheetByPayrollPeriod(payrollPeriod);

            if(timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(payrollPeriod);
            }

            if (checkFinalizedState && timesheet.IsFinalized)
            {
                throw new CannotExportNotFinalizedTimesheetException(payrollPeriod);

            }

            var timesheetCSVModel = _adapter.Adapt(timesheet);

            return timesheetCSVModel;
        } 
    }
}

using Timesheet.Application.Employees.Queries;
using Timesheet.Application.Timesheets.Queries;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class ExportTimesheetService : IExportTimesheetService
    {
        private readonly IQueryTimesheet _timesheets;
        private readonly ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails,ExternalTimesheetCSVEntryModel> _externalAdapter;
        private readonly ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel> _webAdapter;
        private readonly ITimesheetCSVFormatter _formatter;
        private readonly ITimesheetCSVWriter _csvWriter;
        private readonly string _externalDestinationBasePath;

        private readonly string _paylocityFileName = "Paylocity_Timesheet_";

        public ExportTimesheetService(IQueryTimesheet timesheets,
            ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails,ExternalTimesheetCSVEntryModel> externalAdapter,
            ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel> adapter,
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
            _webAdapter = adapter;
            _externalAdapter = externalAdapter;
            _formatter = formatter;
            _csvWriter = csvWriter;
        }

        public async Task ExportToExternal(string payrollPeriod)
        {
            string csv = await GetTimesheetAsCsv<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel>(payrollPeriod, true, _externalAdapter, true);
            string outpuPath = Path.Combine(_externalDestinationBasePath, $"{_paylocityFileName}{payrollPeriod}");
            _csvWriter.SetPath(outpuPath);
            await _csvWriter.Write(csv);
        }

        public async Task<string> ExportToWeb(string payrollPeriod) 
            => await GetTimesheetAsCsv<TimesheetEntryDetails,TimesheetCSVEntryModel>(payrollPeriod, false, _webAdapter, false);

        private async Task<string> GetTimesheetAsCsv<TEntryDetail, TEntryCsvModel>(string payrollPeriod, bool checkFinalizedState, ITimesheetToCSVModelAdapter<TEntryDetail, TEntryCsvModel> adapter, bool ignoreHolidays)
        {
            if (payrollPeriod is null)
            {
                return string.Empty;
            }

            var timesheet = await GetDataToExport<TEntryDetail>(payrollPeriod, checkFinalizedState, ignoreHolidays);
            var timesheetCSVModel = adapter.Adapt(timesheet);
            var csv = _formatter.Format(timesheetCSVModel);
            return csv;
        }

        private async Task<AllEmployeesTimesheet<TEntryDetail>> GetDataToExport<TEntryDetail>(string payrollPeriod, bool checkFinalizedState, bool ignoreHolidays)
        {
            var timesheet = await _timesheets.GetAllEmployeeTimesheetByPayrollPeriod<TEntryDetail>(payrollPeriod, ignoreHolidays);

            if(timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(payrollPeriod);
            }

            if (checkFinalizedState && timesheet.IsFinalized)
            {
                throw new CannotExportNotFinalizedTimesheetException(payrollPeriod);

            }

            return timesheet;
        } 
    }
}

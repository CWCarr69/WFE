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
        private readonly ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel> _externalAdapter;
        private readonly ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel> _webAdapter;
        private readonly ITimesheetCSVFormatter _formatter;
        private readonly ITimesheetCSVWriter _csvWriter;
        private readonly string _externalDestinationBasePath;

        private readonly string _paylocityFileName = "Paylocity_Timesheet_";

        public ExportTimesheetService(IQueryTimesheet timesheets,
            ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel> externalAdapter,
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

        public async Task ExportAdaptedReviewToExternal(string payrollPeriod)
        {
            string csv = await GetTimesheetAsCsv(payrollPeriod, _externalAdapter);
            string outpuPath = Path.Combine(_externalDestinationBasePath, $"{_paylocityFileName}{payrollPeriod}");
            _csvWriter.SetPath(outpuPath);
            await _csvWriter.Write(csv);
        }

        public async Task<string> ExportAdaptedReviewToWeb(string payrollPeriod)
        => await GetTimesheetAsCsv(payrollPeriod, _externalAdapter);

        public async Task<string> ExportRawReviewToWeb(string payrollPeriod, string? department, string? employeeId)
            => await GetTimesheetAsCsv(payrollPeriod, department, employeeId, _webAdapter);

        private async Task<string> GetTimesheetAsCsv(string payrollPeriod, ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel> adapter)
        {
            if (payrollPeriod is null)
            {
                return string.Empty;
            }

            var timesheet = await _timesheets.GetAllTimesheetEntriesByPayrollPeriod(payrollPeriod);

            if (timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(payrollPeriod);
            }

            if (!timesheet.IsFinalized)
            {
                throw new CannotExportNotFinalizedTimesheetException(payrollPeriod);

            }
            return AdaptDataAndFormat(adapter, timesheet);
        }

        private async Task<string> GetTimesheetAsCsv(string payrollPeriod, string? department, string? employeeId, ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel> adapter)
        {
            if (payrollPeriod is null)
            {
                return string.Empty;
            }

            var timesheet = await _timesheets.GetAllTimesheetEntriesByPayrollPeriodAndCriteria(payrollPeriod, department, employeeId);

            if (timesheet is null)
            {
                throw new EntityNotFoundException<TimesheetHeader>(payrollPeriod);
            }

            return AdaptDataAndFormat<TimesheetEntryDetails, TimesheetCSVEntryModel>(adapter, timesheet);
        }

        private string AdaptDataAndFormat<TEntryDetail, TEntryCsvModel>(ITimesheetToCSVModelAdapter<TEntryDetail, TEntryCsvModel> adapter, AllEmployeesTimesheet<TEntryDetail> timesheet)
        {
            var timesheetCSVModel = adapter.Adapt(timesheet);
            var csv = _formatter.Format(timesheetCSVModel);
            return csv;
        }
    }
}

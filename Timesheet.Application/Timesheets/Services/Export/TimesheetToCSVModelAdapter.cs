using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class TimesheetToCSVModelAdapter : ITimesheetToCSVModelAdapter<TimesheetEntryDetails, TimesheetCSVEntryModel>
    {
        public TimesheetCSVModel<TimesheetCSVEntryModel> Adapt(AllEmployeesTimesheet<TimesheetEntryDetails> timesheet)
        {

            var timesheetCSVModel = new TimesheetCSVModel<TimesheetCSVEntryModel>();

            foreach (var entry in timesheet.Entries)
            {
                timesheetCSVModel.Entries.Add(Adapt(entry));
            }

            return timesheetCSVModel;
        }

        private TimesheetCSVEntryModel Adapt(TimesheetEntryDetails entry)
        {
            return new TimesheetCSVEntryModel
            {
                EmployeeId = entry.EmployeeId,
                FullName = entry.Fullname,
                WorkDate = entry.WorkDate.ToShortDateString(),
                PayrollCode = entry.PayrollCode,
                Quantity = entry.Quantity,
                Description = entry.Description,
                ServiceOrder = entry.ServiceOrderNumber,
                ServiceOrderDescription = entry.ServiceOrderDescription,
                Job = entry.JobNumber,
                JobDescription = entry.JobDescription,
                JobTask = entry.JobTaskNumber,
                LaborCode = entry.LaborCode,
                Customer = entry.CustomerNumber,
                ProfitCenter = entry.ProfitCenterNumber,
                WorkArea = entry.WorkArea
            };
        }
    }

    internal class ExternalTimesheetToCSVModelAdapter : ITimesheetToCSVModelAdapter<ExternalTimesheetEntryDetails, ExternalTimesheetCSVEntryModel>
    {
        public TimesheetCSVModel<ExternalTimesheetCSVEntryModel> Adapt(AllEmployeesTimesheet<ExternalTimesheetEntryDetails> timesheet)
        {
            var timesheetCSVModel = new TimesheetCSVModel<ExternalTimesheetCSVEntryModel>();

            foreach (var entry in timesheet.Entries)
            {
                timesheetCSVModel.Entries.Add(Adapt(entry));
            }

            return timesheetCSVModel;
        }

        private ExternalTimesheetCSVEntryModel Adapt(ExternalTimesheetEntryDetails entry)
        {
            return new ExternalTimesheetCSVEntryModel
            {
                No = entry.No,
                PayrollCodeId = entry.PayrollCodeId,
                Hours = entry.Hours,
                CC1 = entry.CC1,
                Job_Code = entry.Job_Code,
                Begin_Date = entry.Begin_Date,
                End_Date = entry.End_Date
            };
        }
    }
}

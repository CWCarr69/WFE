using Timesheet.Domain.Models.Timesheets;
using Timesheet.Domain.ReadModels.Timesheets;

namespace Timesheet.Application.Timesheets.Services.Export
{
    internal class TimesheetToCSVModelAdapter : ITimesheetToCSVModelAdapter
    {
        public TimesheetCSVModel Adapt(AllEmployeesTimesheet timesheet)
        {

            var timesheetCSVModel = new TimesheetCSVModel();

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
}

using Timesheet.Domain.Models.Timesheets;
using Timesheet.Models.Referential;

namespace Timesheet.Domain.ReadModels.Timesheets
{
    public class EmployeeTimesheet
    {
        public string NextTimesheetId { get; set; }
        //{
        //    get
        //    {
        //        return GetTimesheetPayrollPeriod(EndDate.AddDays(1));
        //    }
        //}

        public string PreviousTimesheetId { get; set; }
        //{
        //    get
        //    {
        //        return GetTimesheetPayrollPeriod(StartDate.AddDays(-1));
        //    }
        //}
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string TimesheetId { get; set; }
        public string PayrollPeriod { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ApproverComment { get; set; }
        public string EmployeeComment { get; set; }
        public TimesheetStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string PayrollCode { get; set; }
        public double TotalHours { get; set; }
        public TimesheetType Type { get; set; }
        public IEnumerable<EmployeeTimesheetEntry> Entries { get; set; } = new List<EmployeeTimesheetEntry>();
        public IEnumerable<EmployeeTimesheetEntry> EntriesWithoutTimeoffs => Entries
        ?.Where(t => 
            t.PayrollCodeId == (int) TimesheetFixedPayrollCodeEnum.REGULAR
            || t.PayrollCodeId == (int) TimesheetFixedPayrollCodeEnum.OVERTIME
        ) ?? new List<EmployeeTimesheetEntry>();


        //private string GetTimesheetPayrollPeriod(DateTime datetime)
        //{
        //    return Type == TimesheetType.WEEKLY
        //        ? TimesheetHeader.CreateWeeklyTimesheet(datetime).PayrollPeriod
        //        : TimesheetHeader.CreateMonthlyTimesheet(datetime).PayrollPeriod;
        //}
    }
}

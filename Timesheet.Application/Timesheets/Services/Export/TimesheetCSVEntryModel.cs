namespace Timesheet.Application.Timesheets.Services.Export
{
    public class TimesheetCSVEntryModel
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string WorkDate { get; set; }
        public string PayrollCode { get; set; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public string ServiceOrder { get; set; } = String.Empty;
        public string ServiceOrderDescription { get; set; } = String.Empty;
        public string Job { get; set; } = String.Empty;
        public string JobDescription { get; set; } = String.Empty;
        public string JobTask { get; set; } = String.Empty;
        public string LaborCode { get; set; } = String.Empty;
        public string Customer { get; set; } = String.Empty;
        public string ProfitCenter { get; set; } = String.Empty;
        public string WorkArea { get; set; } = String.Empty;
    }

    public class ExternalTimesheetCSVEntryModel
    {
        public string No { get; set; }
        public string Det => "E";
        public string DetCode { get; set; }
        public double Hours { get; set; }
        public string Amount { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
        public string RateCode { get; set; } = string.Empty;
        public string CC1 { get; set; }
        public string CC2 { get; set; } = string.Empty;
        public string CC3 { get; set; } = string.Empty;
        public string CC4 { get; set; } = string.Empty;
        public string CC5 { get; set; } = string.Empty;
        public string Job_Code { get; set; }
        public string Shift { get; set; } = string.Empty;
        public string Begin_Date { get; set; }
        public string End_Date { get; set; }
        public string Wcc { get; set; } = string.Empty;
        public string Tcode1 { get; set; } = string.Empty;
        public string Tcode2 { get; set; } = string.Empty;
        public string Tcode3 { get; set; } = string.Empty;
        public string Tcode4 { get; set; } = string.Empty;
        public int Sequence => 1;
        public string CheckTyp { get; set; } = string.Empty;
    }
}
namespace Timesheet.Domain.ReadModels.Timesheets;

public class EmployeeTimesheetEntry
{
    public string Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime WorkDate { get; set; }
    public string PayrollCode { get; set; }
    public double Quantity { get; set; }
    public string CustomerNumber{ get; set; }
    public string JobNumber { get; private set; }
    public string JobDescription { get; private set; }
    public string JobTaskNumber { get; private set; }
    public string JobTaskDescription { get; private set; }
    public string LaborCode { get; private set; }
    public string ServiceOrderNumber { get; private set; }
    public string ServiceOrderDescription { get; private set; }
    public string ProfitCenterNumber { get; private set; }
    public string Department { get; private set; }
    public string Description { get; private set; }
    public bool OutOffCountry { get; private set; }
    public string WorkArea => OutOffCountry ? "Out of country" : "In state";
    public string Status { get; set; }
}

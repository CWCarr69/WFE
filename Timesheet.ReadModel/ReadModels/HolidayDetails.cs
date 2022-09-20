namespace Timesheet.ReadModel.ReadModels
{
    public class HolidayDetails
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsRecurrent { get; set; }
    }
}

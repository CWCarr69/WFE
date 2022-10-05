namespace Timesheet.Domain.ReadModels.Holidays
{
    public class HolidayDetails
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsRecurrent { get; set; }
    }
}

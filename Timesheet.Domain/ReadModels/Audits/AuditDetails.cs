namespace Timesheet.Domain.ReadModels.Settings
{
    public class AuditDetails
    {
        public string Entity { get; set; }
        public string EntityId { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string AuthorId { get; set; }
        public string Data { get; set; }
    }
}

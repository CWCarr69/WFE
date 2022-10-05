namespace Timesheet.Domain.Models
{
    public class Audit : Entity
    {
        public Audit(string id) : base(id)
        {
        }

        public string Entity { get; set; }
        public string EntityId { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string CreatorId { get; set; }
        public string Data { get; set; }

    }
}

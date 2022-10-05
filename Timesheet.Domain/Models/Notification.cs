namespace Timesheet.Domain.Models
{
    public class Notification : Entity
    {
        public Notification(string id) : base(id)
        {
        }

        public int Population { get; private set; }
        public SubDomainType Group { get; private set; }
        public string Action { get; private set; }
    }
}

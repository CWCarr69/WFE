namespace Timesheet.Domain
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        protected AggregateRoot(string id) : base(id)
        {
        }

        protected void RaiseDomainEvent(IDomainEvent @event)
        {
            if (!this._domainEvents.Contains(@event))
            {
                this._domainEvents.Add(@event);
            }
        }

        public ICollection<IDomainEvent> GetDomainEvents() => _domainEvents;
        public void ClearDomainEvents() => _domainEvents.Clear();

    }
}

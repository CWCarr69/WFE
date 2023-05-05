using Timesheet.Domain;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Shared
{
    public abstract class BaseEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
        private readonly IUnitOfWork _unitOfWork;


        protected BaseEventHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public abstract Task HandleEvent(TEvent @event);

        public async Task Handle(TEvent @event, CancellationToken token)
        {
            await this.HandleEvent(@event);
            await this._unitOfWork.CompleteAsync(token);
        }
    }
}
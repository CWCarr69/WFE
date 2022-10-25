using Newtonsoft.Json;
using Timesheet.Domain;
using Timesheet.Domain.Models.Audits;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application.Shared
{
    public class AuditHandler : IAuditHandler
    {
        private readonly IWriteRepository<Audit> _auditRepository;

        public AuditHandler(IWriteRepository<Audit> auditRepository)
        {
            this._auditRepository = auditRepository;
        }

        public void LogCommand<TEntity, TCommand>(TEntity entity, TCommand command, CommandActionType auditType, string userId)
            where TEntity : Entity 
            where TCommand : ICommand
        {
            var audit = new Audit(Entity.GenerateId())
            {
                EntityId = entity.Id,
                Entity = entity.GetType().Name,
                Action = command.GetType().Name,
                Type = auditType.ToString(),
                Data = JsonConvert.SerializeObject(command),
                Date = DateTime.Now,
                AuthorId = userId,
            };

            _auditRepository.Add(audit);
        }
    }
}

﻿using Newtonsoft.Json.Linq;
using Timesheet.Application.Employees.Services;
using Timesheet.Application.Shared;
using Timesheet.Domain;
using Timesheet.Domain.Exceptions;
using Timesheet.Domain.Models.Employees;
using Timesheet.Domain.Repositories;

namespace Timesheet.Application
{
    internal abstract class BaseCommandHandler<TEntity, TCommand> : ICommandHandler<TCommand>
        where TEntity : Entity
        where TCommand : ICommand
    {
        private readonly IEmployeeReadRepository _employeeReadRepository;
        private readonly IAuditHandler _auditHandler;
        private readonly IDispatcher _eventDispatcher;
        private readonly IUnitOfWork _transaction;
        private IEmployeeHabilitation _employeeHabilitation;

        protected TEntity RelatedAuditableEntity { get; set; }

        private IEnumerable<IDomainEvent> _events = new List<IDomainEvent>();
        private IEnumerable<IDomainEvent> Events
        {
            get => _events;
            set
            {
                if (value != null || value.Any())
                {
                    _events = value;
                }
            }
        }

        public BaseCommandHandler(
            IEmployeeReadRepository employeeReadRepository,
            IAuditHandler auditHandler, 
            IDispatcher dispatcher,
            IUnitOfWork unitOfWork,
            IEmployeeHabilitation employeeHabilitation)
        {
            _employeeReadRepository = employeeReadRepository;
            _auditHandler = auditHandler;
            _eventDispatcher = dispatcher;
            _transaction = unitOfWork;
            _employeeHabilitation = employeeHabilitation;
        }

        public abstract Task<IEnumerable<IDomainEvent>> HandleCoreAsync(TCommand command, CancellationToken token);

        public async Task HandleAsync(TCommand command, CancellationToken token)
        {
            if(command.Author?.Id is null)
            {
                throw new CannotExecuteCommandWithoutAuthorException(typeof(TCommand).Name);
            }

            if(command is null)
            {
                throw new Exception("Should set command before calling command Handler");
            }

            try
            {
                Events = await HandleCoreAsync(command, token);

                if (RelatedAuditableEntity is not null)
                {
                    RelatedAuditableEntity.UpdateMetadataOnModification(command.Author?.Id);
                    await _auditHandler.LogCommand(RelatedAuditableEntity, command, command.ActionType(), command.Author?.Id);
                }

                if (this.Events.Any() && _eventDispatcher is not null)
                {
                    //This call eventHandler. the eventHandlers are not supposed to complete the transaction
                    await this.PublishEvents(token);
                }

                await _transaction.CompleteAsync(token);
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task PublishEvents(CancellationToken token) => await this._eventDispatcher.Publish(Events, token);

        protected async Task<EmployeeRoleOnData> GetCurrentEmployeeRoleOnData(BaseCommand command, Employee? employee)
        {
            return await _employeeHabilitation.GetEmployeeRoleOnData( command.Author?.Id, employee?.Id, command.Author?.IsAdministrator ?? false);
        }
    }
}

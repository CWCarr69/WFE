using Microsoft.Extensions.DependencyInjection;
using Timesheet.Domain;

namespace Timesheet.Application
{
    public class Dispatcher : IDispatcher
    {
        private readonly HandlersConfiguration _configuration;
        private readonly IServiceProvider _service;

        public Dispatcher(HandlersConfiguration configuration, IServiceProvider service)
        {
            _configuration = configuration;
            _service = service;
        }

        public async Task Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
            if (@event is null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            dynamic handler = GetHandler(
                @event.GetType(),
                _configuration.EventHandlerRegistry,
                $"Event {nameof(@event)} is not registered.",
                $"Event handler for {nameof(@event)} is not registered.");

            await handler.Handle(@event as dynamic);
        }

        public async Task Publish<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : IDomainEvent
        {
            foreach(var @event in events)
            {
                await Publish(@event);
            }
        }

        public async Task RunCommand<TCommand>(TCommand command, string authorId, CancellationToken token)
            where TCommand : ICommand
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.AuthorId = authorId;

            dynamic handler = GetHandler(
                command.GetType(),
                _configuration.CommandHandlerRegistry,
                $"Command {nameof(command)} is not registered.",
                $"Command handler for {nameof(command)} is not registered.");

            await handler.HandleAsync(command, token);
        }

        public async Task RunSubCommand<TCommand>(TCommand command, IDictionary<string, object> context, string authorId, CancellationToken token)
            where TCommand : ICommand
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.AuthorId = authorId;

            dynamic handler = GetHandler(
                command.GetType(),
                _configuration.CommandHandlerRegistry,
                $"Command {nameof(command)} is not registered.",
                $"Command handler for {nameof(command)} is not registered.");

            handler.SetParentCommandContext(context);
            await handler.HandleCoreAsync(command, token);
        }

        private dynamic GetHandler(Type handledType,
            IReadOnlyDictionary<Type, Type> registry,
            string messageIfNotHandledTypeNotRegistered,
            string messageIfHandlerNotRegistered)
        {
            if (!registry.TryGetValue(handledType, out var handlerType))
            {
                throw new ArgumentException(messageIfNotHandledTypeNotRegistered);
            }

            dynamic handler = GetService(messageIfHandlerNotRegistered, handlerType);

            return handler;
        }

        private dynamic GetService(string messageIfServiceNotRegistered, Type? handlerType)
        {
            var handler = _service.GetService(handlerType);
            if (handler == null)
            {
                throw new ArgumentException(messageIfServiceNotRegistered);
            }

            return handler;
        }
    }
}

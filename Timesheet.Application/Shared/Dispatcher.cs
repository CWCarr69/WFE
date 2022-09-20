using Microsoft.Extensions.DependencyInjection;

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

        public void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
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

            handler.Handle(@event as dynamic);
        }

        public void Publish<TDomainEvent>(IEnumerable<TDomainEvent> events) where TDomainEvent : IDomainEvent
        {
            foreach(var @event in events)
            {
                Publish(@event);
            }
        }

        public async Task RunCommand<TCommand>(TCommand command, CancellationToken token)
            where TCommand : ICommand
        {
            using var scopedService = _service.CreateScope();
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            dynamic handler = GetHandler(
                command.GetType(),
                _configuration.CommandHandlerRegistry,
                $"Command {nameof(command)} is not registered.",
                $"Command handler for {nameof(command)} is not registered.");

            await handler.HandleAsync(command, token);
        }

        public async Task RunSubCommand<TCommand>(TCommand command, IDictionary<string, object> context, CancellationToken token)
            where TCommand : ICommand
        {
            using var scopedService = _service.CreateScope();
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            dynamic handler = GetHandler(
                command.GetType(),
                _configuration.CommandHandlerRegistry,
                $"Command {nameof(command)} is not registered.",
                $"Command handler for {nameof(command)} is not registered.");

            handler.SetParentCommandContext(context);
            await handler.HandleAsync(command, token);
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

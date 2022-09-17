using Microsoft.Extensions.DependencyInjection;

namespace Timesheet.Application
{
    public class HandlersConfiguration
    {
        private readonly IServiceProvider _service;

        private readonly Dictionary<Type, Type> _commandHandlerRegistry = new();
        private readonly Dictionary<Type, Type> _eventHandlerRegistry = new();

        public IReadOnlyDictionary<Type, Type> CommandHandlerRegistry => _commandHandlerRegistry;
        public IReadOnlyDictionary<Type, Type> EventHandlerRegistry => _eventHandlerRegistry;

        public HandlersConfiguration(IServiceProvider services)
        {
            this._service = services;
        }

        public void RegisterHandlers()
        {
            RegisterHandlers(
                typeof(IEventHandler<>),
                typeof(IDomainEvent),
                (type, handler) => TryRegisterEventHandlers(type, handler));

            RegisterHandlers(
                typeof(ICommandHandler<>),
                typeof(ICommand),
                (type, handler) => TryRegisterCommandHandlers(type, handler));
        }

        private void RegisterHandlers(Type handlerBaseType, Type handledBaseType, Action<Type, Type> register)
        {
            var _handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s =>
                                s.GetTypes()
                                    .Where(t =>
                                        t.GetInterfaces()
                                            .Any(i => i.IsGenericType &&
                                                 i.GetGenericTypeDefinition().Equals(handlerBaseType))));

            var _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => !t.IsInterface && t.IsAssignableTo(handledBaseType));

            foreach (var handlerType in _handlerTypes)
            {
                using var scope = _service.CreateScope();
                var handler = scope.ServiceProvider.GetService(handlerType);
                if (handler == null)
                {
                    continue;
                }
                foreach (var type in _types)
                {
                    var handlerInterfaceType = handlerType.GetInterfaces().FirstOrDefault();
                    if (handlerInterfaceType?.GetGenericArguments()[0] == type)
                    {
                        register(type, handlerType);
                    }
                }
            }
        }

        public void TryRegisterEventHandlers(Type type, Type eventhandler)
        {
            TryRegisterHandlers(type, eventhandler, typeof(IEventHandler<>), typeof(IDomainEvent), _eventHandlerRegistry);
        }

        public void TryRegisterCommandHandlers(Type type, Type commandHandler)
        {
            TryRegisterHandlers(type, commandHandler, typeof(ICommandHandler<>), typeof(ICommand), _commandHandlerRegistry);
        }

        private void TryRegisterHandlers(Type handledType, Type hander, Type genericBaseType, Type handledBaseType, IDictionary<Type, Type> registry)
        {
            var isHandler = hander
                            .GetInterfaces()
                            .Any(i => i.IsGenericType &&
                                    i.GetGenericTypeDefinition().Equals(genericBaseType));

            if (isHandler && handledType.IsAssignableTo(handledBaseType))
            {
                registry.TryAdd(handledType, hander);
            }
        }
    }
}

using Commodity.Interfaces;
using Ninject;
using System;
using Commodity.Common;
using System.Linq;
using Commodity.Domain.Core.Interfaces;
using MongoDB.Driver;

namespace Commodity.Domain.Core
{
    public class StartupAsServer : IStartUpAsServer
    {
        private readonly IKernel _kernel;
        
        public StartupAsServer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Start()
        {
            // create bus
            //_kernel.Bind<IEventStorage>().To<InMemoryEventStorage>();
            _kernel.Bind<IEventStreamNameResolver>().To<EventStreamNameResolver>();
            _kernel.Bind<IEventStore>().To<EventStore>();
            _kernel.Bind<IEventPublisher>().To<EventPublisher>();
            _kernel.Bind<IAggregateRepository>().To<AggregateRepository>();

            // Mongo
            _kernel.Bind<MongoClient>().ToSelf().WithConstructorArgument("connectionString", "mongodb://localhost:27017");
            _kernel.Bind<IMongoDatabase>().ToMethod(ctx => ctx.Kernel.Get<MongoClient>().GetDatabase("events")).Named("Events");



            // find all event handlers and bind to self.
            var allEventHandlers = AppDomain.CurrentDomain.GetAssemblies().FindTypesImplementingInterface(typeof(IEventHandler<>));
            foreach (var eventHandlerType in allEventHandlers)
            {
                var interfaces = eventHandlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>)).ToArray();
                _kernel.Bind(interfaces).To(eventHandlerType);
            }
        }

        public void Stop()
        {
        }
    }
}

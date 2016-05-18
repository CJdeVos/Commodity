using Commodity.Interfaces;
using Ninject;
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
        }

        public void Stop()
        {
        }
    }
}

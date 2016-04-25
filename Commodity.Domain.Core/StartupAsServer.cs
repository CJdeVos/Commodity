using Commodity.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using Commodity.Common;
using System.Linq;
using Commodity.Domain.Core.Events;
using Commodity.Domain.Core.Interfaces;
using MongoDB.Bson.Serialization;
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

            // Find all event handlers and bind to self.
            var allEventHandlers = AppDomain.CurrentDomain.GetAssemblies().FindTypesImplementingInterface(typeof(IEventHandler<>));
            foreach (var eventHandlerType in allEventHandlers)
            {
                var interfaces = eventHandlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>)).ToArray();
                _kernel.Bind(interfaces).To(eventHandlerType);
            }


            //CommoditySerializer.RegisterSerializer(typeof(IAggregateEvent), new AggregateEventSerializer(), 500);
            CommoditySerializer.RegisterSerializer((f)=>true, new DefaultSerializer());


            //var commoditySerializerResolver = new CommoditySerializerResolver((f) => {
            //    var csType = typeof (ICommoditySerializer<>);
            //    var gcsType = csType.MakeGenericType(f);

            //    return _kernel.Get(gcsType);
            //});

            

            ////commoditySerializerResolver.AddR
            //_kernel.Bind<IResolveCommoditySerializer>().ToConstant(commoditySerializerResolver);
            //commoditySerializerResolver.AddSerializer<StandardCommoditySerializer>((t)=>true, 500);

            // Find all CommoditySerializer<>s and add them
            // 
            
            //commoditySerializerResolver.Serialize(typeof(IEnumerable<>)).With<AggregateEventSerializer>(); 

            //commoditySerializerResolver.AddSerializer(typeof(Created<>)).With<AggregateEventSerializer>();

            //commoditySerializerResolver.AddSerializer<AggregateEventSerializer>(typeof(IAggregateEvent));
            //commoditySerializerResolver.RegisterSerializer<AggregateEventSerializer>(Guid.NewGuid(), typeof(IAggregateEvent));

            //commoditySerializerResolver.Register(new FallbackSerializer(), new SerializerId(Guid.NewGuid()))
            //    .To(typeof(IAggregateEvent))
            //    .To(typeof(object));

            // find all ICommoditySerializer(s)
            //var allCommoditySerializers = AppDomain.CurrentDomain.GetAssemblies().FindTypesImplementingInterface(typeof(ICommoditySerializer<>));
            //foreach (var commoditySerializerType in allCommoditySerializers)
            //{
            //    // Bind for kernel loading
            //    var interfaces = commoditySerializerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommoditySerializer<>)).ToArray();
            //    _kernel.Bind(interfaces).To(commoditySerializerType);

            //    //commoditySerializerResolver.AddSerializer(, 500);
            //}

            BsonSerializer.RegisterSerializer(typeof(IAggregateEvent), new ForwardToCommoditySerializer<IAggregateEvent>());
        }

        public void Stop()
        {
        }
    }
}

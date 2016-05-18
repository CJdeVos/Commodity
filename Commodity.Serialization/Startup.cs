using Commodity.Interfaces;
using System;
using System.Reflection;
using Commodity.Common;
using Commodity.Serialization.Serializers;
using Commodity.Serialization.TypeResolver;
using Ninject;

namespace Commodity.Serialization
{
    public class StartupSerialization : IStartUpAsServer
    {
        private readonly IKernel _kernel;

        public StartupSerialization(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Start()
        {
            CommodityTypeResolver typeResolver = new CommodityTypeResolver();
            
            // Find all Types with attribute CommoditySerializable
            var allTypesWithSerializableAttribute = AppDomain.CurrentDomain.GetAssemblies().FindTypesWithAttribute<CommoditySerializableAttribute>();
            foreach (var type in allTypesWithSerializableAttribute)
            {
                CommoditySerializableAttribute attr = type.GetCustomAttribute<CommoditySerializableAttribute>();
                typeResolver.Register(type, attr.UniqueId);
            }
            typeResolver.Register(typeof(object), "object");

            // Commodity Serializers
            // 1) Type (priority 100)
            CommoditySerializer.RegisterSerializer((f) => typeof(Type).IsAssignableFrom(f), new TypeSerializer(typeResolver), 100);
            CommoditySerializer.RegisterSerializer((f) => f == typeof(String), new StringSerializer(), 100);
            CommoditySerializer.RegisterSerializer((f) => f.IsValueType, new ValueTypeSerializer(), 100);

            // 2) * 
            CommoditySerializer.RegisterSerializer((f) => true, new DefaultSerializer());
        }

        public void Stop()
        {
            //throw new NotImplementedException();
        }
    }
}

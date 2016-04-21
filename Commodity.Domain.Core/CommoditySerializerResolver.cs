//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Commodity.Domain.Core
//{
//    public class CommoditySerializerResolver : IResolveCommoditySerializer
//    {
//        private class DescendingSerializerComparer<T> : IComparer<T>
//        {
//            public int Compare(T x, T y)
//            {
//                return Comparer<T>.Default.Compare(y, x);
//            }
//        }

//        private class SerializerInfo
//        {
//            public Func<Type, bool> Applies { get; set; }
//            public Type SerializerType { get; set; }
//        }

//        private class CommoditySerializerWrapper<T> : ICommoditySerializer where T: ICommoditySerializer<>
//        {
//            public object Deserialize(ICommodityReader reader)
//            {
//                throw new NotImplementedException();
//            }

//            public T Serialize<T>(ICommodityWriter writer, object o)
//            {
//                throw new NotImplementedException();
//            }
//        }

//        private readonly Dictionary<RuntimeTypeHandle, Type> _resolvedTypeSerializers = new Dictionary<RuntimeTypeHandle, Type>();
//        private readonly SortedList<int, SerializerInfo> _serializers = new SortedList<int, SerializerInfo>(new DescendingSerializerComparer<int>());
        
//        private readonly Func<Type, object> _fnGetSerializer;

//        public CommoditySerializerResolver(Func<Type, object> fnGetSerializer)
//        {
//            _fnGetSerializer = fnGetSerializer;
//        }

//        public void AddSerializer<TSerializer>(Func<Type, bool> appliesFunc, int priority) where TSerializer : ICommoditySerializer
//        {
//            _serializers.Add(priority, new SerializerInfo() { Applies = appliesFunc, SerializerType = typeof(TSerializer)});
//        }

//        public void AddSerializer<TSerializer>(ICommoditySerializer<TSerializableType> serializer) where TSerializer : ICommoditySerializer<>
//        {
            
//        }

//        public object Resolve(Type t)
//        {
//            // check resolved types first
//            if (_resolvedTypeSerializers.ContainsKey(t.TypeHandle))
//                return _resolvedTypeSerializers[t.TypeHandle];
//            var serializer = _serializers.Values.FirstOrDefault(s => s.Applies(t));
//            if (serializer == null){
//                throw new Exception("No serializer resolved for type.");
//            }
//            _resolvedTypeSerializers.Add(t.TypeHandle, serializer.SerializerType);
//            return _fnGetSerializer(t);
//        }

//        public ICommoditySerializer<T> Resolve<T>()
//        {
//            return Resolve(typeof(T)) as ICommoditySerializer<T>;
//        }

//        public void Serialize(ICommodityWriter writer, Type nominalType, object value)
//        {
//            object o = this.Resolve(nominalType);
//            if (o == null)
//            {
//                throw new NotImplementedException(String.Format("No CommoditySerializer found for object of type {0}", nominalType.FullName));
//            }
//            ((dynamic) o).Serialize(writer, value);
//        }

//    }
//}
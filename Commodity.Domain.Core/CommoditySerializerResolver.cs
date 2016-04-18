using System;

namespace Commodity.Domain.Core
{
    public class CommoditySerializerResolver : IResolveCommoditySerializer
    {
        private readonly Func<Type, object> _fnGetSerializer;

        public CommoditySerializerResolver(Func<Type, object> fnGetSerializer)
        {
            _fnGetSerializer = fnGetSerializer;
        }

        public object Resolve(Type t)
        {
            return _fnGetSerializer(t);
        }

        public ICommoditySerializer<T> Resolve<T>()
        {
            return Resolve(typeof(T)) as ICommoditySerializer<T>;
        }

        public void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            object o = this.Resolve(nominalType);
            if (o == null)
            {
                throw new NotImplementedException(String.Format("No CommoditySerializer found for object of type {0}", nominalType.FullName));
            }
            ((dynamic) o).Serialize(writer, value);
        }
    }
}
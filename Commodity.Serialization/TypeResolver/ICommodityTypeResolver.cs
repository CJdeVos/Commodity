using System;

namespace Commodity.Serialization.TypeResolver
{
    public interface ICommodityTypeResolver
    {
        Type GetTypeFromHandle(string handle);
        string GetHandleFromType(Type type);
    }
}
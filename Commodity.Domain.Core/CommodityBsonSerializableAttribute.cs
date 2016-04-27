using System;
using System.Collections.Generic;
using System.Linq;

namespace Commodity.Domain.Core
{
    public class CommodityBsonSerializableAttribute : Attribute
    {
        public CommodityBsonSerializableAttribute(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        public string UniqueId { get; private set; }
    }

    public interface ICommodityBsonTypeResolver
    {
        Type GetTypeFromHandle(string handle);
        string GetHandleFromType(Type type);
    }

    public sealed class CommodityBsonTypeResolver : ICommodityBsonTypeResolver
    {
        private readonly Dictionary<RuntimeTypeHandle, string> _typeToId = new Dictionary<RuntimeTypeHandle, string>();
        private readonly Dictionary<string, RuntimeTypeHandle> _idToType = new Dictionary<string, RuntimeTypeHandle>();
        public void Register(Type type, string handle)
        {
            if(_idToType.ContainsKey(handle))
                throw new Exception("UniqueId already there...");

            RuntimeTypeHandle typeHandle = type.TypeHandle;
            if(_typeToId.ContainsKey(typeHandle))
                throw new Exception("Handle already there...");
            
            _typeToId.Add(typeHandle, handle);
            _idToType.Add(handle, typeHandle);
        }

        public string GetHandleFromType(Type type)
        {
            var actualType =
                type.IsGenericType
                    ? type.GetGenericTypeDefinition()
                    : type;

            RuntimeTypeHandle handle = actualType.TypeHandle;
            if (!_typeToId.ContainsKey(handle))
                throw new Exception(String.Format("Type <{0}> is not registered as a CommodityBsonSerializable.", actualType.ToString()));
            string handleForActualType = _typeToId[handle];

            if (type.IsGenericType) // genericType<argumentTypes,...>
            {
                return String.Format("{0}<{1}>",
                    handleForActualType,
                    String.Join(",", type.GetGenericArguments().Select(GetHandleFromType))
                    );
            }
            return handleForActualType;
        }

        public Type GetTypeFromHandle(string handle)
        {
            // read string until <
            var genericStartIndex = handle.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);
            if (genericStartIndex > 0)
            {
                // is generic
                var genericEndIndex = handle.LastIndexOf(">", StringComparison.InvariantCultureIgnoreCase);
                if(genericEndIndex<0) throw new Exception("Cannot have generic start without end...");

                string genericTypeHandle = handle.Substring(0, genericStartIndex);

                // improve on argument type finding (by character, keep counter for additional generic types...
                string genericArgumentHandles = handle.Substring(genericStartIndex + 1, genericEndIndex - genericStartIndex - 1);
                List<string> genericArguments = new List<string>();
                int counter = 0;
                string current = "";
                foreach (char c in genericArgumentHandles)
                {
                    if (c.Equals('<'))
                    {
                        counter++;
                    }
                    else if (c.Equals('>'))
                    {
                        counter--;
                    }
                    else if (c.Equals(',') && counter==0)
                    {
                        genericArguments.Add(current);
                        current = "";
                        continue;
                    }
                    current += c;
                }
                genericArguments.Add(current);

                //string[] genericArgumentTypeHandles = genericArgumentHandles.Split(new [] {","}, StringSplitOptions.None);

                return
                    GetTypeFromHandle(genericTypeHandle)
                        .MakeGenericType(genericArguments.Select(GetTypeFromHandle).ToArray());
            }

            if (!_idToType.ContainsKey(handle))
                throw new Exception("Handle is not registered as a CommodityBsonSerializable.");
            return Type.GetTypeFromHandle(_idToType[handle]);
        }
    }

    public interface ICommodityBsonMap
    {
        void Map(ICommodityBsonMapper mapper);
    }

    public interface ICommodityBsonMapper
    {

    }

    public class DefaultCommodityBsonMapping : ICommodityBsonMapper
    {
        
    }

}

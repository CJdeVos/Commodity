using System;
using System.Linq;
using System.Reflection;
using Commodity.Domain.Core;
using MongoDB.Bson;

namespace Commodity.Serialization.Serializers
{
    public class DefaultSerializer : ICommoditySerializer
    {
        public void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var actualType = value.GetType();
            writer.WriteStartOfObject();
            writer.WriteName("t");
            CommoditySerializer.Serialize(writer, typeof(Type), actualType);
            //writer.WriteType(actualType);
            writer.WriteName("v");
            writer.WriteStartOfObject();
            SerializeAttributes(writer, actualType, value);
            writer.WriteEndOfObject();
            writer.WriteEndOfObject();
        }

        

        public object Deserialize(ICommodityReader reader, Type nominalType)
        {
            BsonType bsonType = reader.ReadBsonType();
            switch (bsonType)
            {
                case BsonType.Null:
                    return null;
                    break;
                case BsonType.Document:
                    reader.ReadStartOfObject();
                    var typeName = reader.ReadName();
                    Type actualType = CommoditySerializer.Deserialize<Type>(reader);
                    // Create type
                    object instance = InstantiateType(actualType);
                    var valueName = reader.ReadName(); // is always "v"
                    reader.ReadStartOfObject();
                    DeserializeAttributes(reader, actualType, instance);
                    reader.ReadEndOfObject();
                    reader.ReadEndOfObject();
                    return instance;
                    break;
            }
            throw new Exception("nothing to deserialize?");
            return null;
        }

        private object InstantiateType(Type type)
        {
            return Activator.CreateInstance(type);
        }

        private CommodityMemberInfoSerializer GetMemberSerializeAction(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propInfo = (PropertyInfo)memberInfo;
                if(propInfo.CanRead && propInfo.CanWrite)
                    return new CommodityPropertyInfoSerializer(memberInfo);
            }
            return null;
        }

        private CommodityMemberInfoSerializer[] GetSerializableMemberInfos(Type nominalType)
        {
            return nominalType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Select(GetMemberSerializeAction)
                .Where(d=>d!=null)
                .ToArray();
        }

        private void SerializeAttributes(ICommodityWriter writer, Type nominalType, object value)
        {
            var serializableMembers = GetSerializableMemberInfos(nominalType);
            foreach (CommodityMemberInfoSerializer memberInfoSerializer in serializableMembers)
            {
                object memberValue = memberInfoSerializer.GetValue(value);
                writer.WriteName(memberInfoSerializer.Name);
                CommoditySerializer.Serialize(writer, memberInfoSerializer.MemberType, memberValue);
            }
        }

        private void DeserializeAttributes(ICommodityReader reader, Type actualType, object instance)
        {
            var serializableMembers = GetSerializableMemberInfos(actualType);

            //var bsonType = ;
            // loop over reader
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                string name = reader.ReadName();
                // find name in the serializableMembers
                CommodityMemberInfoSerializer memberInfoSerializer = serializableMembers.FirstOrDefault(member => member.Name == name);
                if (memberInfoSerializer == null)
                    throw new Exception("Member not found.");

                // read value (call into deserializer)
                object value = CommoditySerializer.Deserialize(reader, memberInfoSerializer.MemberType);
                memberInfoSerializer.SetValue(instance, value);
            }
        }
    }
}
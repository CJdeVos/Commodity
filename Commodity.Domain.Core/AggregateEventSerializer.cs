using System;
using System.Linq;
using System.Reflection;
using Commodity.Interfaces;
using MongoDB.Bson;

namespace Commodity.Domain.Core
{
    //public class AggregateEventSerializer : CommodityBsonSerializer<IAggregateEvent>
    //{
    //    public override void Serialize(ICommodityWriter writer, IAggregateEvent o)
    //    {
    //        writer.WriteStartOfObject();
    //        writer.WriteName("t");
    //        writer.WriteString(o.GetType().FullName);
    //        writer.WriteName("i");
    //        CommodityBsonSerializer.Serialize(writer, o.GetType(), o);
    //        writer.WriteEndOfObject();
    //    }

    //    public override IAggregateEvent Deserialize(ICommodityReader reader)
    //    {


    //        throw new NotImplementedException();
    //    }
    //}

    public class TypeSerializer : CommoditySerializer<Type>
    {
        private readonly ICommodityBsonTypeResolver _typeResolver;
        public TypeSerializer(ICommodityBsonTypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public override Type Deserialize(ICommodityReader reader)
        {
            string handle = reader.ReadString();
            return _typeResolver.GetTypeFromHandle(handle);
        }

        public override void Serialize(ICommodityWriter writer, Type o)
        {
            string handle = _typeResolver.GetHandleFromType(o);
            writer.WriteString(handle);
        }
        
    }

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
            CommodityBsonSerializer.Serialize(writer, typeof(Type), actualType);
            //writer.WriteType(actualType);
            writer.WriteName("v");
            writer.WriteStartOfObject();
            SerializeAttributes(writer, actualType, value);
            writer.WriteEndOfObject();
            writer.WriteEndOfObject();
        }

        public object Deserialize(ICommodityReader reader, Type nominalType)
        {
            BsonType bsonType = reader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Null:
                    return null;
                    break;
                case BsonType.Document:
                    reader.ReadStartOfObject();
                    var typeName = reader.ReadName();
                    Type actualType = CommodityBsonSerializer.Deserialize<Type>(reader);
                    var valueName = reader.ReadName();
                    reader.ReadStartOfObject();
                    DeserializeAttributes(reader, actualType);
                    reader.ReadEndOfObject();
                    reader.ReadEndOfObject();
                    break;
            }
            return null;
        }

        internal class MemberInfoAndAction
        {
            public MemberInfoAndAction(
                MemberInfo memberInfo, 
                Action<ICommodityWriter, MemberInfo, object> writeAction, 
                Func<ICommodityReader, MemberInfo, object> readAction)
            {
                MemberInfo = memberInfo;
                WriteAction = writeAction;
                ReadAction = readAction;
            }
            public MemberInfo MemberInfo { get; private set; }
            public Action<ICommodityWriter, MemberInfo, object> WriteAction { get; private set; }
            public Func<ICommodityReader, MemberInfo, object> ReadAction { get; private set; }
        }

        private MemberInfoAndAction GetMemberSerializeAction(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propInfo = (PropertyInfo)memberInfo;
                if(propInfo.CanRead && propInfo.CanWrite)
                    return new MemberInfoAndAction(memberInfo, SerializeMemberAsProperty, DeserializeMemberAsProperty);
            }
            return null;
        }

        private MemberInfoAndAction[] GetSerializableMemberInfos(Type nominalType)
        {
            return nominalType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Select(GetMemberSerializeAction)
                .Where(d=>d!=null)
                .ToArray();
        }

        private void SerializeAttributes(ICommodityWriter writer, Type nominalType, object value)
        {
            var serializableMembers = GetSerializableMemberInfos(nominalType);
            foreach (MemberInfoAndAction memberAndAction in serializableMembers)
            {
                writer.WriteName(memberAndAction.MemberInfo.Name);
                memberAndAction.WriteAction(writer, memberAndAction.MemberInfo, value);
            }
        }

        private void DeserializeAttributes(ICommodityReader reader, Type actualType)
        {
            var serializableMembers = GetSerializableMemberInfos(actualType);
            foreach (MemberInfoAndAction memberAndAction in serializableMembers)
            {
                string name = reader.ReadName();
                object value = memberAndAction.ReadAction(reader, memberAndAction.MemberInfo);
                //memberAndAction.Action(writer, memberAndAction.MemberInfo, value);
            }
        }
        private static readonly Action<ICommodityWriter, MemberInfo, object> SerializeMemberAsProperty = (writer, memberInfo, value) =>
        {
            var propertyInfo = (PropertyInfo)memberInfo;
            var propertyValue = propertyInfo.GetValue(value);
            if (propertyValue == null)
                writer.WriteNull();
            else
                writer.WriteString(propertyValue.ToString());
        };

        private static readonly Func<ICommodityReader, MemberInfo, object> DeserializeMemberAsProperty = (reader, memberInfo) =>
        {
            var bsonType = reader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                reader.ReadNull();
                return null;
            }
            return reader.ReadString();
        };
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Commodity.Domain.Core;
using Commodity.Interfaces;
using MongoDB.Bson;

namespace Commodity.Domain.Core
{
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

    public class StringSerializer : CommoditySerializer<String>
    {
        public override string Deserialize(ICommodityReader reader)
        {
            return reader.ReadString();
        }

        public override void Serialize(ICommodityWriter writer, string value)
        {
            writer.WriteString(value);
        }
    }

    public abstract class CommodityMemberInfoSerializer
    {
        protected CommodityMemberInfoSerializer(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }
        protected MemberInfo MemberInfo { get; private set; }

        public string Name
        {
            get { return MemberInfo.Name; }
        }

        public abstract Type MemberType{get;}

        public abstract object GetValue(object instance);
        public abstract void SetValue(object instance, object value);
    }

    public class CommodityPropertyInfoSerializer : CommodityMemberInfoSerializer
    {
        public CommodityPropertyInfoSerializer(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        protected PropertyInfo PropertyInfo { get { return (PropertyInfo) MemberInfo; } }

        public override object GetValue(object instance)
        {
            return PropertyInfo.GetValue(instance);
        }

        public override void SetValue(object instance, object value)
        {
            PropertyInfo.SetValue(instance, value);
        }

        public override Type MemberType
        {
            get { return PropertyInfo.PropertyType; }
        }
    }

    public class ValueTypeSerializer : ICommoditySerializer
    {
        public void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            // must be a struct serializer
            throw new NotImplementedException();
        }

        public object Deserialize(ICommodityReader reader, Type nominalType)
        {
            throw new NotImplementedException();
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
            BsonType bsonType = reader.ReadBsonType();
            switch (bsonType)
            {
                case BsonType.Null:
                    return null;
                    break;
                case BsonType.Document:
                    reader.ReadStartOfObject();
                    var typeName = reader.ReadName();
                    Type actualType = CommodityBsonSerializer.Deserialize<Type>(reader);
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
                CommodityBsonSerializer.Serialize(writer, memberInfoSerializer.MemberType, memberValue);
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
                object value = CommodityBsonSerializer.Deserialize(reader, memberInfoSerializer.MemberType);
                memberInfoSerializer.SetValue(instance, value);
            }
        }
        //private static readonly Action<ICommodityWriter, MemberInfo, object> SerializeMemberAsProperty = (writer, memberInfo, value) =>
        //{
        //    var propertyInfo = (PropertyInfo)memberInfo;
        //    var propertyValue = propertyInfo.GetValue(value);
        //    if (propertyValue == null)
        //        writer.WriteNull();
        //    else
        //        writer.WriteString(propertyValue.ToString());
        //};

        //private static readonly Func<ICommodityReader, MemberInfo, object> DeserializeMemberAsProperty = (reader, memberInfo) =>
        //{
        //    var bsonType = reader.GetCurrentBsonType();
        //    if (bsonType == BsonType.Null)
        //    {
        //        reader.ReadNull();
        //        return null;
        //    }
        //    return reader.ReadString();
        //};
    }
}
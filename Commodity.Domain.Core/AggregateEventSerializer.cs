using System;
using System.Linq;
using System.Reflection;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class AggregateEventSerializer : CommoditySerializer<IAggregateEvent>
    {
        public override void Serialize(ICommodityWriter writer, IAggregateEvent o)
        {
            writer.WriteStartOfObject();
            writer.WriteName("t");
            writer.WriteString(o.GetType().FullName);
            writer.WriteName("i");
            CommoditySerializer.Serialize(writer, o.GetType(), o);
            writer.WriteEndOfObject();
        }

        public override IAggregateEvent Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultSerializer : ICommoditySerializer
    {
        public void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            writer.WriteStartOfObject();
            writer.WriteName("t");
            writer.WriteType(nominalType);
            writer.WriteName("v");
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartOfObject();
                // loop over serializable attributes
                SerializeAttributes(writer, nominalType, value);
                writer.WriteEndOfObject();
            }
            writer.WriteEndOfObject();
        }

        public object Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }

        internal class MemberInfoAndAction
        {
            public MemberInfoAndAction(MemberInfo memberInfo, Action<ICommodityWriter, MemberInfo, object> action)
            {
                MemberInfo = memberInfo;
                Action = action;
            }
            public MemberInfo MemberInfo { get; private set; }
            public Action<ICommodityWriter, MemberInfo, object> Action { get; private set; }
        }

        private MemberInfoAndAction GetMemberSerializeAction(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propInfo = (PropertyInfo)memberInfo;
                if(propInfo.CanRead && propInfo.CanWrite)
                    return new MemberInfoAndAction(memberInfo, _serializeMemberAsProperty);
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
                memberAndAction.Action(writer, memberAndAction.MemberInfo, value);
            }
        }

        private static readonly Action<ICommodityWriter, MemberInfo, object> _serializeMemberAsProperty = (writer, memberInfo, value) =>
        {
            var propertyInfo = (PropertyInfo)memberInfo;
            var propertyValue = propertyInfo.GetValue(value);
            if (propertyValue == null)
                writer.WriteNull();
            else
                writer.WriteString(propertyValue.ToString());
        };
    }
}
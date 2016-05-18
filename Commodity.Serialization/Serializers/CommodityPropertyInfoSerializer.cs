using System;
using System.Reflection;

namespace Commodity.Serialization.Serializers
{
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
}
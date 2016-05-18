using System;
using System.Reflection;

namespace Commodity.Serialization.Serializers
{
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
}
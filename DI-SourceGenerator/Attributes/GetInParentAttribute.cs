using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetInParent : Attribute
    {
        public GetInParent() {}
    }
}
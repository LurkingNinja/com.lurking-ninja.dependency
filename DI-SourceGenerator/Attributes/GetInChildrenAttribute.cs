using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetInChildren : Attribute
    {
        public GetInChildren() {}
    }
}
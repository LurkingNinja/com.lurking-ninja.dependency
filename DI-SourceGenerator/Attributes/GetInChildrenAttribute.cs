namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetInChildren : Attribute
    {
        public GetInChildren() {}
    }
}
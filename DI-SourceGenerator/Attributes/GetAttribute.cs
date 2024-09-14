namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Get : Attribute
    {
        public Get() {}
    }
}
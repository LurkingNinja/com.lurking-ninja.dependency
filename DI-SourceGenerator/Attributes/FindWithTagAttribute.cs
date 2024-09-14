namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FindWithTag : Attribute
    {
        public FindWithTag(string tagName) {}
    }
}
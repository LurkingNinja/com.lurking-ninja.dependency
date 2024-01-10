using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FindWithTag : Attribute
    {
        public FindWithTag(string tagName) {}
    }
}
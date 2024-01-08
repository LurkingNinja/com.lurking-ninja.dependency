using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FindByTag : Attribute
    {
        public FindByTag(string tagName) {}
    }
}
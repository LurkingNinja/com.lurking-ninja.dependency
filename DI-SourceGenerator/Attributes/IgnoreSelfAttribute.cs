using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IgnoreSelf : Attribute
    {
        public IgnoreSelf(string gameObjectName) {}
    }
}
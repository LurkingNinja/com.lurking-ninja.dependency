using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Find : Attribute
    {
        public Find(string gameObjectName) {}
    }
}
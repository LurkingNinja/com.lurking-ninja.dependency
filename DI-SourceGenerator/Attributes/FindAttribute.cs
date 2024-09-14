namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Find : Attribute
    {
        public Find(string gameObjectName) {}
    }
}
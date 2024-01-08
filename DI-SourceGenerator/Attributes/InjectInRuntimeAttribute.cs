using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectInRuntime : Attribute
    {
        public InjectInRuntime(string gameObjectName) {}
    }
}
using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectInEditor : Attribute
    {
        public InjectInEditor(string gameObjectName) {}
    }
}
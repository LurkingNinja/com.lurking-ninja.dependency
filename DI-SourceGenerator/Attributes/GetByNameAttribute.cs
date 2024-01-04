using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetByName : Attribute
    {
        public GetByName(string gameObjectName) {}
    }
}
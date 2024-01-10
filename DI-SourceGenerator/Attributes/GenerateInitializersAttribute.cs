using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateInitializers : Attribute
    {
        public GenerateInitializers() {}
    }
}
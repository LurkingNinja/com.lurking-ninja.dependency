namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateInitializers : Attribute
    {
        public GenerateInitializers() {}
    }
}
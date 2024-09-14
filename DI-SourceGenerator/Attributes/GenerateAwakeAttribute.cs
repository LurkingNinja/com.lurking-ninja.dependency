namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateAwake : Attribute
    {
        public GenerateAwake() {}
    }
}
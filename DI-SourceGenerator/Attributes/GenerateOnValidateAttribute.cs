namespace LurkingNinja.Dependency.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateOnValidate : Attribute
    {
        public GenerateOnValidate() {}
    }
}
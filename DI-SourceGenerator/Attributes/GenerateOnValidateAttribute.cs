using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateOnValidate : Attribute
    {
        public GenerateOnValidate() {}
    }
}
using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateAwake : Attribute
    {
        public GenerateAwake() {}
    }
}
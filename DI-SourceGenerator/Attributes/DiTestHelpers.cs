using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DiTestHelpers : Attribute
    {
        public DiTestHelpers() {}
    }
}
using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetInAssets : Attribute
    {
        public GetInAssets(string assetSearchPath) {}
    }
}
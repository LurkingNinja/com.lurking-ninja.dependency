﻿using System;

namespace LurkingNinja.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SkipNullCheck : Attribute
    {
        public SkipNullCheck() {}
    }
}
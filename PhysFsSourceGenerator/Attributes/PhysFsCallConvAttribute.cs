using System;
using System.Collections.Generic;
using System.Text;

namespace Icculus.PhysFS.NET.SourceGeneration;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
public class PhysFsCallConvAttribute : Attribute
{
    public Type[]? CallConvs { get; }

    public PhysFsCallConvAttribute(Type[]? callConv)
    {
        CallConvs = callConv;
    }
}

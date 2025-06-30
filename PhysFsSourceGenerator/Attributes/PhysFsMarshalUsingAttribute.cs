using System;

namespace Icculus.PhysFS.NET.SourceGeneration;

[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
public class PhysFsMarshalUsingAttribute : Attribute
{
    public Type MarshallerType { get; }
    public bool ShouldBeFreed { get; } 

    public PhysFsMarshalUsingAttribute(Type marshaller, bool shouldBeFreed = true)
    {
        MarshallerType = marshaller;
        ShouldBeFreed = shouldBeFreed;
    }
}

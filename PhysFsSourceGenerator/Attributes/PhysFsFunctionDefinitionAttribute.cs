using System;

namespace Icculus.PhysFS.NET.SourceGeneration;

[AttributeUsage(AttributeTargets.Method)]
public class PhysFsFunctionDefinitionAttribute : Attribute
{
    public string EntryPoint { get; }
    public Type[] ArgumentTypes { get; }
    public Type ReturnType { get; }

    public PhysFsFunctionDefinitionAttribute(string entryPoint, Type[] argumentTypes, Type returnType)
    {
        EntryPoint = entryPoint;
        ArgumentTypes = argumentTypes;
        ReturnType = returnType;
    }
}

using System;

//TODO: Find out why BadImageFormatException is thrown if this attribute is defined in the portable assembly (only for methods, for properties it works fine!)

/// <summary>
/// Marks a method or a property (or all methods and properties of a type)
/// so that no static replacements will be performed in the method or property body.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property)]
public class SkipStaticReplacementsAttribute : Attribute
{
}

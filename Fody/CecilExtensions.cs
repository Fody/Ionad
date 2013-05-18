using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public static class CecilExtensions
{
    public static void RemoveStaticReplacementAttribute(this ICustomAttributeProvider definition)
    {
        var customAttributes = definition.CustomAttributes;

        var attribute = customAttributes.FirstOrDefault(x => x.AttributeType.Name == "StaticReplacementAttribute");

        if (attribute != null)
        {
            customAttributes.Remove(attribute);
        }
    }

    public static CustomAttribute GetStaticReplacementAttribute(this ICustomAttributeProvider value)
    {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == "StaticReplacementAttribute");
    }

    public static IEnumerable<MethodDefinition> MethodsWithBody(this TypeDefinition type)
    {
        return type.Methods.Where(x => x.Body != null);
    }

    public static IEnumerable<PropertyDefinition> ConcreteProperties(this TypeDefinition type)
    {
        return type.Properties.Where(x => (x.GetMethod == null || !x.GetMethod.IsAbstract) && (x.SetMethod == null || !x.SetMethod.IsAbstract));
    }
}
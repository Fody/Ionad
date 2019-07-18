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

    static MethodReference CloneMethodWithDeclaringType(MethodDefinition methodDef, TypeReference declaringTypeRef)
    {
        if (!declaringTypeRef.IsGenericInstance || methodDef == null)
        {
            return methodDef;
        }

        var methodRef = new MethodReference(methodDef.Name, methodDef.ReturnType, declaringTypeRef)
        {
            CallingConvention = methodDef.CallingConvention,
            HasThis = methodDef.HasThis,
            ExplicitThis = methodDef.ExplicitThis
        };

        foreach (var paramDef in methodDef.Parameters)
        {
            methodRef.Parameters.Add(new ParameterDefinition(paramDef.Name, paramDef.Attributes, paramDef.ParameterType));
        }

        foreach (var genParamDef in methodDef.GenericParameters)
        {
            methodRef.GenericParameters.Add(new GenericParameter(genParamDef.Name, methodRef));
        }

        return methodRef;
    }

    public static MethodReference ReferenceMethod(this TypeReference typeRef, Func<MethodDefinition, bool> methodSelector)
    {
        return CloneMethodWithDeclaringType(typeRef.Resolve().Methods.FirstOrDefault(methodSelector), typeRef);
    }

    public static MethodReference ReferenceMethod(this TypeReference typeRef, MethodDefinition method)
    {
        return ReferenceMethod(typeRef, m => 
            m.Name == method.Name && Matches(m, method)
            );
    }

    private static bool Matches(IMethodSignature left, IMethodSignature right)
    {
        return ReturnMatches(left, right) &&
               left.Parameters.Count == right.Parameters.Count &&
               Enumerable.Zip(left.Parameters, right.Parameters, Matches).All(x => x);
    }
    
    private static bool Matches(ParameterDefinition left, ParameterDefinition right)
    {
        if (left.ParameterType == right.ParameterType)
            return true;
        if (left.ParameterType.IsGenericParameter && right.ParameterType.IsGenericParameter)
            return true;

        return false;
    }

    private static bool Matches(TypeReference left, TypeReference right)
    {
        if (left.FullName == right.FullName)
            return true;
        if (left.IsGenericParameter && right.IsGenericParameter)
            return true;

        return false;
    }
    
    private static bool ReturnMatches(IMethodSignature left, IMethodSignature right)
    {
        if (left.ReturnType.FullName == right.ReturnType.FullName &&
            Enumerable.Zip(left.ReturnType.GenericParameters, right.ReturnType.GenericParameters, Matches).All(x => x)
        )
            return true;

        if (left.ReturnType.IsGenericParameter && right.ReturnType.IsGenericParameter)
            return true;

        return false;
    }
}
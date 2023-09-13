using Mono.Cecil;

public static class CecilExtensions
{
    public static void RemoveStaticReplacementAttribute(this ICustomAttributeProvider definition)
    {
        var customAttributes = definition.CustomAttributes;

        var attribute = customAttributes.FirstOrDefault(_ => _.AttributeType.Name == "StaticReplacementAttribute");

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
        return type.Methods.Where(_ => _.Body != null);
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
            methodRef.Parameters.Add(new(paramDef.Name, paramDef.Attributes, paramDef.ParameterType));
        }

        foreach (var genParamDef in methodDef.GenericParameters)
        {
            methodRef.GenericParameters.Add(new(genParamDef.Name, methodRef));
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

    static bool Matches(IMethodSignature left, IMethodSignature right)
    {
        return ReturnMatches(left, right) &&
               left.Parameters.Count == right.Parameters.Count &&
               left.Parameters.Zip(right.Parameters, Matches).All(_ => _);
    }

    static bool Matches(ParameterDefinition left, ParameterDefinition right)
    {
        if (left.ParameterType == right.ParameterType)
            return true;
        if (left.ParameterType.IsGenericParameter && right.ParameterType.IsGenericParameter)
            return true;

        return false;
    }

    static bool Matches(TypeReference left, TypeReference right)
    {
        if (left.FullName == right.FullName)
            return true;
        if (left.IsGenericParameter && right.IsGenericParameter)
            return true;

        return false;
    }

    static bool ReturnMatches(IMethodSignature left, IMethodSignature right)
    {
        if (left.ReturnType.FullName == right.ReturnType.FullName &&
            left.ReturnType.GenericParameters.Zip(right.ReturnType.GenericParameters, Matches).All(_ => _)
        )
            return true;

        if (left.ReturnType.IsGenericParameter && right.ReturnType.IsGenericParameter)
        {
            return true;
        }

        return false;
    }
}
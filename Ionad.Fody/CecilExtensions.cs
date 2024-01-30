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

    public static CustomAttribute GetStaticReplacementAttribute(this ICustomAttributeProvider value) =>
        value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == "StaticReplacementAttribute");

    public static IEnumerable<MethodDefinition> MethodsWithBody(this TypeDefinition type) =>
        type.Methods.Where(_ => _.Body != null);

    public static IEnumerable<PropertyDefinition> ConcreteProperties(this TypeDefinition type) =>
        type.Properties.Where(x => (x.GetMethod == null || !x.GetMethod.IsAbstract) && (x.SetMethod == null || !x.SetMethod.IsAbstract));

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

    public static MethodReference ReferenceMethod(this TypeReference typeRef, Func<MethodDefinition, bool> methodSelector) =>
        CloneMethodWithDeclaringType(typeRef.Resolve().Methods.FirstOrDefault(methodSelector), typeRef);

    public static MethodReference ReferenceMethod(this TypeReference typeRef, MethodDefinition method) =>
        ReferenceMethod(
            typeRef,
            _ => _.Name == method.Name && Matches(_, method)
        );

    static bool Matches(IMethodSignature left, IMethodSignature right) =>
        ReturnMatches(left, right) &&
        left.Parameters.Count == right.Parameters.Count &&
        left.Parameters.Zip(right.Parameters, Matches).All(_ => _);

    static bool Matches(ParameterDefinition left, ParameterDefinition right) =>
        left.ParameterType == right.ParameterType ||
        left.ParameterType.IsGenericParameter &&
        right.ParameterType.IsGenericParameter;

    static bool Matches(TypeReference left, TypeReference right) =>
        left.FullName == right.FullName ||
        left.IsGenericParameter &&
        right.IsGenericParameter;

    static bool ReturnMatches(IMethodSignature left, IMethodSignature right) =>
        left.ReturnType.FullName == right.ReturnType.FullName &&
        left.ReturnType.GenericParameters.Zip(right.ReturnType.GenericParameters, Matches).All(_ => _) || left.ReturnType.IsGenericParameter && right.ReturnType.IsGenericParameter;
}
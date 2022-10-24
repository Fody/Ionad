using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver:
    BaseModuleWeaver
{
    public override void Execute()
    {
        var types = ModuleDefinition.GetTypes()
            .ToList();
        var replacements = FindReplacements(types);

        if (replacements.Count == 0)
        {
            WriteInfo("No Static Replacements found");
        }
        else
        {
            ProcessAssembly(types, replacements);
            RemoveAttributes(replacements.Values);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield break;
    }

    static Dictionary<TypeDefinition, TypeDefinition> FindReplacements(IEnumerable<TypeDefinition> types)
    {
        var replacements = new Dictionary<TypeDefinition, TypeDefinition>();

        foreach (var type in types)
        {
            var replacement = type.GetStaticReplacementAttribute();
            if (replacement == null)
            {
                continue;
            }

            var replacementType = ((TypeReference)replacement.ConstructorArguments[0].Value).Resolve();

            replacements.Add(replacementType, type);
        }

        return replacements;
    }

    void ProcessAssembly(IEnumerable<TypeDefinition> types, Dictionary<TypeDefinition, TypeDefinition> replacements)
    {
        foreach (var type in types)
        {
            foreach (var method in type.MethodsWithBody())
            {
                ReplaceCalls(method.Body, replacements);
            }

            foreach (var property in type.ConcreteProperties())
            {
                if (property.GetMethod != null)
                {
                    ReplaceCalls(property.GetMethod.Body, replacements);
                }

                if (property.SetMethod != null)
                {
                    ReplaceCalls(property.SetMethod.Body, replacements);
                }
            }
        }
    }

    void ReplaceCalls(MethodBody body, Dictionary<TypeDefinition, TypeDefinition> replacements)
    {
        body.SimplifyMacros();

        var calls = body.Instructions.Where(i => i.OpCode == OpCodes.Call);

        foreach (var call in calls)
        {
            var originalMethodReference = (MethodReference)call.Operand;
            var originalMethodDefinition = originalMethodReference.Resolve();
            var declaringTypeReference = originalMethodReference.DeclaringType;
            var declaringTypeDefinition = declaringTypeReference.Resolve();

            if (!originalMethodDefinition.IsStatic || !replacements.ContainsKey(declaringTypeDefinition))
            {
                continue;
            }

            var replacementTargetType = replacements[declaringTypeDefinition];
            if (IsReplacementOnSelf(body, replacementTargetType))
            {
                // Skip replacement if the replacement call site is in the replacement target class
                // this allows the Replacement class to be able to call the original class methods
                // Allowing decorator style programming in Ionad.
                continue;
            }

            var replacementTypeReference = ModuleDefinition.ImportReference(replacementTargetType);
            if (declaringTypeReference.IsGenericInstance)
            {
                var declaringGenericType = (GenericInstanceType)declaringTypeReference;
                var genericType = new GenericInstanceType(replacementTypeReference);
                foreach (var arg in declaringGenericType.GenericArguments)
                {
                    genericType.GenericArguments.Add(arg);
                }
                replacementTypeReference = ModuleDefinition.ImportReference(genericType);
            }

            var replacementMethod = replacementTypeReference.ReferenceMethod(originalMethodDefinition);

            if (replacementMethod == null)
            {
                WriteError($"Missing '{declaringTypeDefinition.FullName}.{originalMethodDefinition.Name}()' in '{replacementTypeReference.FullName}'");
                continue;
            }

            if (!replacementMethod.Resolve().IsStatic)
            {
                WriteError($"Replacement method '{replacementMethod.FullName}' is not static");
                continue;
            }

            if (originalMethodReference.IsGenericInstance)
            {
                var originalGenericInstanceMethod = (GenericInstanceMethod)originalMethodReference;
                var genericInstanceMethod = new GenericInstanceMethod(replacementMethod);
                foreach (var arg in originalGenericInstanceMethod.GenericArguments)
                {
                    genericInstanceMethod.GenericArguments.Add(arg);
                }

                call.Operand = ModuleDefinition.ImportReference(genericInstanceMethod);
            }
            else
            {
                call.Operand = replacementMethod;
            }
        }

        body.InitLocals = true;
        body.OptimizeMacros();
    }

    /// <summary>
    /// This method is to try to stop replacing calls to the base method from the replacement
    /// class. This allows the replacement class to decorate the base methods.
    /// </summary>
    static bool IsReplacementOnSelf(MethodBody body, TypeDefinition replacementTargetType)
    {
        var methodDeclaringType = body.Method.DeclaringType;

        // We don't want to replace calls in nested classes either, because
        // the C# compiler regularly generates nested classes for certain
        // state machine like methods. For examples:
        //  1) yield return xxx;
        //  2) await Task
        //  3) lambda variable captures
        bool IsNestedClassMethod(TypeDefinition methodType)
        {
            if (methodType == replacementTargetType)
            {
                return true;
            }

            if (methodType.IsNestedPrivate)
            {
                return IsNestedClassMethod(methodType.DeclaringType);
            }
            return false;
        }
        return IsNestedClassMethod(methodDeclaringType);
    }

    static void RemoveAttributes(IEnumerable<TypeDefinition> types)
    {
        foreach (var typeDefinition in types)
        {
            typeDefinition.RemoveStaticReplacementAttribute();
        }
    }

    public override bool ShouldCleanReference => true;
}
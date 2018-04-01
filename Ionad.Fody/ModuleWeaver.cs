using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver:BaseModuleWeaver
{
    public override void Execute()
    {
        var types = ModuleDefinition.GetTypes()
            .ToList();
        var replacements = FindReplacements(types);

        if (replacements.Count == 0)
        {
            LogInfo("No Static Replacements found");
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

    Dictionary<TypeDefinition, TypeDefinition> FindReplacements(IEnumerable<TypeDefinition> types)
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

            var replacementTypeReference = ModuleDefinition.ImportReference(replacements[declaringTypeDefinition]);
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

            var replacementMethod = replacementTypeReference.ReferenceMethod(originalMethodDefinition.Name);

            if (replacementMethod == null)
            {
                LogError($"Missing '{declaringTypeDefinition.FullName}.{originalMethodDefinition.Name}()' in '{replacementTypeReference.FullName}'");
                continue;
            }

            if (!replacementMethod.Resolve().IsStatic)
            {
                LogError($"Replacement method '{replacementMethod.FullName}' is not static");
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

    void RemoveAttributes(IEnumerable<TypeDefinition> types)
    {
        foreach (var typeDefinition in types)
        {
            typeDefinition.RemoveStaticReplacementAttribute();
        }
    }

    public override bool ShouldCleanReference => true;
}
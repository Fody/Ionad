using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public Action<string> LogError { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public string[] DefineConstants { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogError = s => { };
        DefineConstants = new string[0];
    }

    public void Execute()
    {
        var types = ModuleDefinition.GetTypes()
            .ToList();
        var replacements = FindReplacements(types);

        if (replacements.Count == 0)
            LogInfo("No Static Replacements found");
        else
        {
            ProcessAssembly(types, replacements);
            RemoveAttributes(replacements.Values);
        }

        RemoveReference();
    }

    private Dictionary<TypeDefinition, TypeDefinition> FindReplacements(IEnumerable<TypeDefinition> types)
    {
        var replacements = new Dictionary<TypeDefinition, TypeDefinition>();

        foreach (var type in types)
        {
            var replacement = type.GetStaticReplacementAttribute();
            if (replacement == null)
                continue;

            var replacementType = ((TypeReference)replacement.ConstructorArguments[0].Value).Resolve();

            replacements.Add(replacementType, type);
        }

        return replacements;
    }

    private void ProcessAssembly(IEnumerable<TypeDefinition> types, Dictionary<TypeDefinition, TypeDefinition> replacements)
    {
        foreach (var type in types.Where(t => !t.HasSkipStaticReplacementsAttribute()))
        {
            foreach (var method in type.MethodsWithBody().Where(m => !m.HasSkipStaticReplacementsAttribute()))
                ReplaceCalls(method.Body, replacements);

            foreach (var property in type.ConcreteProperties().Where(m => !m.HasSkipStaticReplacementsAttribute()))
            {
                if (property.GetMethod != null)
                    ReplaceCalls(property.GetMethod.Body, replacements);
                if (property.SetMethod != null)
                    ReplaceCalls(property.SetMethod.Body, replacements);
            }
        }
    }

    private void ReplaceCalls(MethodBody body, Dictionary<TypeDefinition, TypeDefinition> replacements)
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
                continue;

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
                LogError(String.Format("Missing '{0}.{1}()' in '{2}'", declaringTypeDefinition.FullName, originalMethodDefinition.Name, replacementTypeReference.FullName));
                continue;
            }

            if (!replacementMethod.Resolve().IsStatic)
            {
                LogError(String.Format("Replacement method '{0}' is not static", replacementMethod.FullName));
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

    private void RemoveAttributes(IEnumerable<TypeDefinition> types)
    {
        foreach (var typeDefinition in types)
        {
            typeDefinition.RemoveStaticReplacementAttribute();
            typeDefinition.RemoveSkipStaticReplacementsAttribute();
        }
    }

    private void RemoveReference()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Ionad");
        if (referenceToRemove == null)
        {
            LogInfo("\tNo reference to 'Ionad.dll' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'Ionad.dll'.");
    }
}
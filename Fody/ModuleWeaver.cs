using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

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
        var types = ModuleDefinition.GetTypes();
        var replacements = FindReplacements(types);

        if (replacements.Count == 0)
            LogInfo("No Static Replacements found");
        else
        {
            ProcessAssembly(types);
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

    private void ProcessAssembly(IEnumerable<TypeDefinition> types)
    {
        foreach (var type in types)
        {
        }
    }

    private void RemoveAttributes(IEnumerable<TypeDefinition> types)
    {
        foreach (var typeDefinition in types)
            typeDefinition.RemoveStaticReplacementAttribute();
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
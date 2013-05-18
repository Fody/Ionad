using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;

public static class AssemblyWeaver
{
    public static Assembly Assembly;

    static AssemblyWeaver()
    {
        BeforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");

#if (!DEBUG)
        BeforeAssemblyPath = BeforeAssemblyPath.Replace("Debug", "Release");
#endif
        AfterAssemblyPath = BeforeAssemblyPath.Replace(".dll", "2.dll");

        File.Copy(BeforeAssemblyPath, AfterAssemblyPath, true);

        var assemblyResolver = new MockAssemblyResolver();
        var moduleDefinition = ModuleDefinition.ReadModule(AfterAssemblyPath);

        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
            AssemblyResolver = assemblyResolver,
            LogError = LogError,
            LogInfo = LogInfo,
            DefineConstants = new[] { "DEBUG" } // Always testing the debug weaver
        };

        weavingTask.Execute();
        moduleDefinition.Write(AfterAssemblyPath);

        Assembly = Assembly.LoadFile(AfterAssemblyPath);
    }

    public static string BeforeAssemblyPath;
    public static string AfterAssemblyPath;

    private static void LogError(string error)
    {
        Errors.Add(error);
    }

    private static void LogInfo(string error)
    {
        Infos.Add(error);
    }

    public static List<string> Errors = new List<string>();
    public static List<string> Infos = new List<string>();
}
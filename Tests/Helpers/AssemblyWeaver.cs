using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Pdb;

public static class AssemblyWeaver
{
    public static Assembly Assembly;

    static AssemblyWeaver()
    {
        BeforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
        var beforePdbPath = Path.ChangeExtension(BeforeAssemblyPath, "pdb");

#if (!DEBUG)
        BeforeAssemblyPath = BeforeAssemblyPath.Replace("Debug", "Release");
        beforePdbPath = beforePdbPath.Replace("Debug", "Release");
#endif
        AfterAssemblyPath = BeforeAssemblyPath.Replace(".dll", "2.dll");
        var afterPdbPath = beforePdbPath.Replace(".pdb", "2.pdb");

        File.Copy(BeforeAssemblyPath, AfterAssemblyPath, true);
        if (File.Exists(beforePdbPath))
            File.Copy(beforePdbPath, afterPdbPath, true);

        var assemblyResolver = new DefaultAssemblyResolver();
        var mockAssemblyResolver = new MockAssemblyResolver();
        var readerParameters = new ReaderParameters { AssemblyResolver = assemblyResolver };
        var writerParameters = new WriterParameters();

        if (File.Exists(afterPdbPath))
        {
            readerParameters.SymbolReaderProvider = new PdbReaderProvider();
            readerParameters.ReadSymbols = true;
            writerParameters.WriteSymbols = true;
        }

        var moduleDefinition = ModuleDefinition.ReadModule(AfterAssemblyPath, readerParameters);

        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
            AssemblyResolver = assemblyResolver,
            LogError = LogError,
            LogInfo = LogInfo,
            DefineConstants = new[] { "DEBUG" } // Always testing the debug weaver
        };

        weavingTask.Execute();
        moduleDefinition.Write(AfterAssemblyPath, writerParameters);

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
using Fody;
#pragma warning disable 618

public partial class ModuleWeaverTests
{
    static TestResult testResult;

    static ModuleWeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }
}
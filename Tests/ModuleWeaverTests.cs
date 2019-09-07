using Fody;
using Xunit.Abstractions;

public partial class ModuleWeaverTests :
    XunitApprovalBase
{
    static TestResult testResult;

    static ModuleWeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    public ModuleWeaverTests(ITestOutputHelper output) :
        base(output)
    {
    }
}
#if(NET46)

using ApprovalTests;
using Fody;
using Xunit;

public partial class ModuleWeaverTests :
    XunitLoggingBase
{
    [Fact]
    public void ClassWithBrokenReplacement()
    {
        Approvals.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithBrokenReplacement"));
    }

    [Fact]
    public void ClassWithDateTime()
    {
        Approvals.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithDateTime"));
    }

    [Fact]
    public void ClassWithGenericMethodUsage()
    {
        Approvals.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericMethodUsage"));
    }

    [Fact]
    public void ClassWithGenericUsage()
    {
        Approvals.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericUsage"));
    }
}

#endif
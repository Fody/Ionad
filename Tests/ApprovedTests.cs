using System.Threading.Tasks;
using Fody;
using Xunit;

public partial class ModuleWeaverTests
{
    [Fact]
    public Task ClassWithBrokenReplacement()
    {
        UniqueForRuntime();
        return Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithBrokenReplacement"));
    }

    [Fact]
    public Task ClassWithDateTime()
    {
        UniqueForRuntime();
        return Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithDateTime"));
    }

    [Fact]
    public Task ClassWithGenericMethodUsage()
    {
        UniqueForRuntime();
        return Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericMethodUsage"));
    }

    [Fact]
    public Task ClassWithGenericUsage()
    {
        UniqueForRuntime();
        return Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericUsage"));
    }
}
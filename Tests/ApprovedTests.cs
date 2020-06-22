using System.Threading.Tasks;
using Fody;
using VerifyTests;
using VerifyXunit;
using Xunit;

public partial class ModuleWeaverTests
{
    [Fact]
    public Task ClassWithBrokenReplacement()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithBrokenReplacement"), settings);
    }

    [Fact]
    public Task ClassWithDateTime()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithDateTime"), settings);
    }

    [Fact]
    public Task ClassWithGenericMethodUsage()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericMethodUsage"), settings);
    }

    [Fact]
    public Task ClassWithGenericUsage()
    {
        var settings = new VerifySettings();
        settings.UniqueForRuntime();
        return Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericUsage"), settings);
    }
}
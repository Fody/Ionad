using Fody;
#pragma warning disable CS0618

[UsesVerify]
public class ModuleWeaverTests
{
    static TestResult testResult;

    static VerifySettings settings;

    static ModuleWeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
        settings = new();
        settings.UniqueForRuntime();
        settings.UniqueForAssemblyConfiguration();
    }

    [Fact]
    public Task ClassWithBrokenReplacement() =>
        Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithBrokenReplacement"), settings);

    [Fact]
    public Task ClassWithDateTime() =>
        Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithDateTime"), settings);

    [Fact]
    public Task ClassWithGenericMethodUsage() =>
        Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericMethodUsage"), settings);

    [Fact]
    public Task ClassWithGenericUsage() =>
        Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericUsage"), settings);

    [Fact]
    public void EnsureHasCanAccessBaseMethodsWithoutStackOverflow()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        Assert.Equal(1, instance.ReplacedCount);
    }

    [Fact]
    public void EnsureHasCanAccessBaseMethodsWithinAYield()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        Assert.Equal(Enumerable.Range(10, 10), instance.Yield);
    }

    [Fact]
    public async Task EnsureHasCanAccessBaseMethodViaLambda()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        Assert.Equal(1, await instance.AsyncWithLambdaReplacement);
    }

    [Fact]
    public async Task EnsureHasCanAccessBaseMethodWorksWithAsyncDecoration()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        Assert.Equal(1, await instance.AsyncDecorator);
    }

    [Fact]
    public void EnsureErrorReported() =>
        Assert.Contains("Replacement method 'System.Void StaticBasicReplacementWithBrokenMethod::SomeMethod()' is not static", testResult.Errors.Select(_ => _.Text));

    [Fact]
    public void MethodUsesDateTime()
    {
        var sample = testResult.GetInstance("ClassWithDateTime");
        var now = sample.GetDateTime();
        Assert.Equal(new DateTime(1978, 1, 13), now);
    }

    [Fact]
    public void PropertyUsesDateTime()
    {
        var sample = testResult.GetInstance("ClassWithDateTime");
        var now = sample.SomeProperty;
        Assert.Equal(new DateTime(1978, 1, 13), now);
    }

    [Fact]
    public void MissingReplacementReportsError() =>
        Assert.Contains("Missing 'System.DateTime.get_Today()' in 'DateTimeReplacement'", testResult.Errors.Select(_ => _.Text));

    [Fact]
    public void EnsureGenericHasBeenReplace()
    {
        var instance = testResult.GetInstance("ClassWithGenericMethodUsage");
        instance.Method();
    }

    [Fact]
    public void EnsureFirstOverloadWithGetsReplaced()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        Assert.Equal(-1, instance.Overloaded0());
    }

    [Fact]
    public void EnsureOverloadWithNoArgumentsWorks()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        Assert.Equal(0, instance.Overloaded1());
    }

    [Fact]
    public void EnsureOverloadWithWithDifferentTypeWorks()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        Assert.Equal(1, instance.Overloaded2());
    }

    [Fact]
    public void EnsureOverloadWithWithDifferentStringWorks()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        Assert.Equal(2, instance.Overloaded3());
    }

    [Fact]
    public void EnsureOverloadGenericReturn()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        var ret = instance.Overloaded4();
        Assert.Equal(0, ret.Count);
    }

    [Fact]
    public void EnsureOverloadGenericParamAndReturn()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        var ret = instance.Overloaded5();
        Assert.Equal(1, ret.Count);
    }

    [Fact]
    public void EnsureHasBeenReplace()
    {
        var instance = testResult.GetInstance("ClassWithGenericUsage");
        instance.Method();
    }
}
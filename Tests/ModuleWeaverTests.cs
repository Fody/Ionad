using Fody;
using TestResult = Fody.TestResult;
#pragma warning disable CS0618

public class ModuleWeaverTests
{
    static TestResult testResult;

    static VerifySettings settings;

    static ModuleWeaverTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll");
        settings = new();
        settings.UniqueForRuntime();
        settings.UniqueForAssemblyConfiguration();
    }

    [Test]
    public async Task ClassWithBrokenReplacement() =>
        await Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithBrokenReplacement"), settings);

    [Test]
    public async Task ClassWithDateTime() =>
        await Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithDateTime"), settings);

    [Test]
    public async Task ClassWithGenericMethodUsage() =>
        await Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericMethodUsage"), settings);

    [Test]
    public async Task ClassWithGenericUsage() =>
        await Verifier.Verify(Ildasm.Decompile(testResult.AssemblyPath, "ClassWithGenericUsage"), settings);

    [Test]
    public async Task EnsureHasCanAccessBaseMethodsWithoutStackOverflow()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        int replacedCount = instance.ReplacedCount;
        await Assert.That(replacedCount).IsEqualTo(1);
    }

    [Test]
    public async Task EnsureHasCanAccessBaseMethodsWithinAYield()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        IEnumerable<int> yielded = instance.Yield;
        await Assert.That(yielded).IsEquivalentTo(Enumerable.Range(10, 10));
    }

    [Test]
    public async Task EnsureHasCanAccessBaseMethodViaLambda()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        int result = await instance.AsyncWithLambdaReplacement;
        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task EnsureHasCanAccessBaseMethodWorksWithAsyncDecoration()
    {
        var instance = testResult.GetInstance("ClassWithBaseAccess");
        int result = await instance.AsyncDecorator;
        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task EnsureErrorReported() =>
        await Assert.That(testResult.Errors.Select(_ => _.Text))
            .Contains("Replacement method 'System.Void StaticBasicReplacementWithBrokenMethod::SomeMethod()' is not static");

    [Test]
    public async Task MethodUsesDateTime()
    {
        var sample = testResult.GetInstance("ClassWithDateTime");
        DateTime now = sample.GetDateTime();
        await Assert.That(now).IsEqualTo(new DateTime(1978, 1, 13));
    }

    [Test]
    public async Task PropertyUsesDateTime()
    {
        var sample = testResult.GetInstance("ClassWithDateTime");
        DateTime now = sample.SomeProperty;
        await Assert.That(now).IsEqualTo(new DateTime(1978, 1, 13));
    }

    [Test]
    public async Task MissingReplacementReportsError() =>
        await Assert.That(testResult.Errors.Select(_ => _.Text))
            .Contains("Missing 'System.DateTime.get_Today()' in 'DateTimeReplacement'");

    [Test]
    public void EnsureGenericHasBeenReplace()
    {
        var instance = testResult.GetInstance("ClassWithGenericMethodUsage");
        instance.Method();
    }

    [Test]
    public async Task EnsureFirstOverloadWithGetsReplaced()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        int result = instance.Overloaded0();
        await Assert.That(result).IsEqualTo(-1);
    }

    [Test]
    public async Task EnsureOverloadWithNoArgumentsWorks()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        int result = instance.Overloaded1();
        await Assert.That(result).IsEqualTo(0);
    }

    [Test]
    public async Task EnsureOverloadWithWithDifferentTypeWorks()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        int result = instance.Overloaded2();
        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task EnsureOverloadWithWithDifferentStringWorks()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        int result = instance.Overloaded3();
        await Assert.That(result).IsEqualTo(2);
    }

    [Test]
    public async Task EnsureOverloadGenericReturn()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        var ret = instance.Overloaded4();
        int count = ret.Count;
        await Assert.That(count).IsEqualTo(0);
    }

    [Test]
    public async Task EnsureOverloadGenericParamAndReturn()
    {
        var instance = testResult.GetInstance("ClassWithOverloads");
        var ret = instance.Overloaded5();
        int count = ret.Count;
        await Assert.That(count).IsEqualTo(1);
    }

    [Test]
    public void EnsureHasBeenReplace()
    {
        var instance = testResult.GetInstance("ClassWithGenericUsage");
        instance.Method();
    }
}

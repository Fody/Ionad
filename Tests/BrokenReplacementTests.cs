using NUnit.Framework;

[TestFixture]
public class BrokenReplacementTests
{
    [Test]
    public void EnsureErrorReported()
    {
        var type = AssemblyWeaver.Assembly.GetType("ClassWithBrokenReplacement");
        Assert.Contains("Replacement method 'System.Void StaticBasicReplacementWithBrokenMethod::SomeMethod()' is not static", AssemblyWeaver.Errors);
    }
}
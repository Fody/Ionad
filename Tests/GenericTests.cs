using Xunit;

public partial class ModuleWeaverTests
{
    [Fact]
    public void EnsureHasBeenReplace()
    {
        var instance = testResult.GetInstance("ClassWithGenericUsage");
        instance.Method();
    }
}
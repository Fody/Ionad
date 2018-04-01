using Xunit;

public partial class ModuleWeaverTests
{
    [Fact]
    public void EnsureGenericHasBeenReplace()
    {
        var instance = testResult.GetInstance("ClassWithGenericMethodUsage");
        instance.Method();
    }
}
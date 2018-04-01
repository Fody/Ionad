using System.Linq;
using Xunit;

public partial class ModuleWeaverTests
{
    [Fact]
    public void EnsureErrorReported()
    {
        Assert.Contains("Replacement method 'System.Void StaticBasicReplacementWithBrokenMethod::SomeMethod()' is not static", testResult.Errors.Select(x=>x.Text));
    }
}
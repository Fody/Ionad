using System.Linq;
using System.Threading.Tasks;
using Xunit;

public partial class ModuleWeaverTests
{
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
}
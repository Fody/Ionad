using Xunit;

public partial class ModuleWeaverTests
{
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
}
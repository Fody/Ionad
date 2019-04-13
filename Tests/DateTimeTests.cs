using System;
using System.Linq;
using Xunit;

public partial class ModuleWeaverTests 
{
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
    public void MissingReplacementReportsError()
    {
        Assert.Contains("Missing 'System.DateTime.get_Today()' in 'DateTimeReplacement'", testResult.Errors.Select(x=>x.Text));
    }
}
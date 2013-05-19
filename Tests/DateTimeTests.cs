using System;
using NUnit.Framework;

[TestFixture]
public class DateTimeTests
{
    private readonly Type sampleClassType;

    public DateTimeTests()
    {
        sampleClassType = AssemblyWeaver.Assembly.GetType("ClassWithDateTime");
    }

    [Test]
    public void MethodUsesDateTime()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.GetDateTime();
        Assert.AreEqual(new DateTime(1978, 1, 13), now);
    }
    [Test]
    public void GenericClass()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.GetDateTime();
        Assert.AreEqual(new DateTime(1978, 1, 13), now);
    }

    [Test]
    public void PropertyUsesDateTime()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.SomeProperty;
        Assert.AreEqual(new DateTime(1978, 1, 13), now);
    }

    [Test]
    public void MissingReplacementReportsError()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        Assert.AreEqual("Missing 'System.DateTime.get_Today()' in 'DateTimeSubstitute'", AssemblyWeaver.Errors[0]);
    }
}
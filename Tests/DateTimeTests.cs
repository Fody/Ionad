using System;
using NUnit.Framework;

[TestFixture]
public class DateTimeTests
{
    private readonly Type sampleClassType;
    private readonly Type sampleClassWithNoReplacementsType;

    public DateTimeTests()
    {
        sampleClassType = AssemblyWeaver.Assembly.GetType("ClassWithDateTime");
        sampleClassWithNoReplacementsType = AssemblyWeaver.Assembly.GetType("ClassWithDateTimeAndNoReplacements");
    }

    [Test]
    public void MethodUsesDateTime()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.GetDateTime();
        Assert.AreEqual(new DateTime(1978, 1, 13), now);
    }

    [Test]
    public void MethodWithSkipStaticReplacementsAttributeUsesRealDateTime()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.NonReplacedGetDateTime();
        Assert.AreEqual(DateTime.Now.Date, now.Date);
    }

    [Test]
    public void PropertyUsesDateTime()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.SomeProperty;
        Assert.AreEqual(new DateTime(1978, 1, 13), now);
    }

    [Test]
    public void PropertyWithSkipStaticReplacementsAttributeUsesRealDateTime()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        var now = sample.NonReplacedSomeProperty;
        Assert.AreEqual(DateTime.Now.Date, now.Date);
    }

    [Test]
    public void MethodsAreSkippedIfStaticReplacementsAttributeInClass()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassWithNoReplacementsType);
        var now = sample.GetDateTime();
        Assert.AreEqual(DateTime.Now.Date, now.Date);
    }

    [Test]
    public void PropertiesAreSkippedIfStaticReplacementsAttributeInClass()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassWithNoReplacementsType);
        var now = sample.SomeProperty;
        Assert.AreEqual(DateTime.Now.Date, now.Date);
    }

    [Test]
    public void MissingReplacementReportsError()
    {
        var sample = (dynamic)Activator.CreateInstance(sampleClassType);
        Assert.Contains("Missing 'System.DateTime.get_Today()' in 'DateTimeReplacement'", AssemblyWeaver.Errors);
    }
}
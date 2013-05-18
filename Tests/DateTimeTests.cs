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
}
using System;
using NUnit.Framework;

[TestFixture]
public class GenericTests
{
    [Test]
    public void EnsureHasBeenReplace()
    {
        var type = AssemblyWeaver.Assembly.GetType("ClassWithGenericUsage");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Method();
    }
}
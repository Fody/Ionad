using System;
using NUnit.Framework;

[TestFixture]
public class GenericMethodTests
{
    [Test]
    public void EnsureHasBeenReplace()
    {
        var type = AssemblyWeaver.Assembly.GetType("ClassWithGenericMethodUsage");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Method();
    }
}
using System;
using NUnit.Framework;

[TestFixture]
public class WithGenericMethodTests
{
    [Test]
    public void EnsureHasBeenReplace()
    {
        var type = AssemblyWeaver.Assembly.GetType("WithGenericMethodUsage");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Method();
    }
}
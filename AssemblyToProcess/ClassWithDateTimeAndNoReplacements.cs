using System;
using Ionad;

[SkipStaticReplacements]
public class ClassWithDateTimeAndNoReplacements
{
    public DateTime GetDateTime()
    {
        return DateTime.Now;
    }

    public DateTime SomeProperty { get { return DateTime.Now; } }
}

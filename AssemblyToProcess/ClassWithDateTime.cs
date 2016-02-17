using System;
using Ionad;

public class ClassWithDateTime
{
    public DateTime GetDateTime()
    {
        return DateTime.Now;
    }

    [SkipStaticReplacements]
    public DateTime NonReplacedGetDateTime()
    {
        return DateTime.Now;
    }

    public DateTime SomeProperty => DateTime.Now;

    [SkipStaticReplacements]
    public DateTime NonReplacedSomeProperty { get { return DateTime.Now; } }

    public DateTime MissingReplacement()
    {
        return DateTime.Today;
    }
}

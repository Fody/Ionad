using System;

public class ClassWithDateTime
{
    public DateTime GetDateTime()
    {
        return DateTime.Now;
    }

    public DateTime SomeProperty { get { return DateTime.Now; } }

    public DateTime MissingReplacement()
    {
        return DateTime.Today;
    }
}
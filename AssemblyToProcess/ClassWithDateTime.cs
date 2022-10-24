public class ClassWithDateTime
{
    public DateTime GetDateTime()
    {
        return DateTime.Now;
    }

    public DateTime SomeProperty => DateTime.Now;

    public DateTime MissingReplacement()
    {
        return DateTime.Today;
    }
}
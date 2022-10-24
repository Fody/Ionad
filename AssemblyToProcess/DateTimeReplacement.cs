using Ionad;

[StaticReplacement(typeof(DateTime))]
public static class DateTimeReplacement
{
    public static DateTime Now => new DateTime(1978, 1, 13);
}
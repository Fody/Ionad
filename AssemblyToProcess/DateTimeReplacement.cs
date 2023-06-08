using Ionad;

[StaticReplacement(typeof(DateTime))]
public static class DateTimeReplacement
{
    public static DateTime Now => new(1978, 1, 13);
}
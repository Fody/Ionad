using System;
using Ionad;

[StaticReplacement(typeof(DateTime))]
public static class DateTimeReplacement
{
    public static DateTime Now { get { return new DateTime(1978, 1, 13); } }
}
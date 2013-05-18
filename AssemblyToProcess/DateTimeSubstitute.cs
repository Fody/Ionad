using System;
using Ionad;

[StaticReplacement(typeof(DateTime))]
public class DateTimeSubstitute
{
    public DateTime Now { get { return new DateTime(1978, 1, 13); } }
}
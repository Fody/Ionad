using System.Collections.Generic;
using Ionad;

[StaticReplacement(typeof(StaticWithOverloads))]
public static class StaticWithOverloadsReplacement
{
    public static int Overloaded(long i)
    {
        return -1;
    }

    public static int Overloaded()
    {
        return 0;
    }

    public static int Overloaded(int i)
    {
        return 1;
    }

    public static int Overloaded(string i)
    {
        return 2;
    }

    public static Dictionary<TKey, TValue> Overloaded<TKey, TValue>()
    {
        return new Dictionary<TKey, TValue>();
    }

    public static Dictionary<TKey, TValue> Overloaded<TKey, TValue>(TKey key, TValue value)
    {
        return new Dictionary<TKey, TValue>(){ {key, value }};
    }
}
using Ionad;

[StaticReplacement(typeof(StaticWithOverloads))]
public static class StaticWithOverloadsReplacement
{
    public static int Overloaded(long i) => -1;

    public static int Overloaded() => 0;

    public static int Overloaded(int i) => 1;

    public static int Overloaded(string i) => 2;

    public static Dictionary<TKey, TValue> Overloaded<TKey, TValue>() => new();

    public static Dictionary<TKey, TValue> Overloaded<TKey, TValue>(TKey key, TValue value) =>
        new()
            { {key, value }};
}
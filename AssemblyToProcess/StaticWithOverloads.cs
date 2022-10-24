public static class StaticWithOverloads
{
    public static int Overloaded(long i)
    {
        throw new NotImplementedException();
    }

    public static int Overloaded()
    {
        throw new NotImplementedException();
    }

    public static int Overloaded(int i)
    {
        throw new NotImplementedException();
    }

    public static int Overloaded(string i)
    {
        throw new NotImplementedException();
    }

    public static Dictionary<TKey, TValue> Overloaded<TKey, TValue>()
    {
        throw new NotImplementedException();
    }

    public static Dictionary<TKey, TValue> Overloaded<TKey, TValue>(TKey key, TValue value)
    {
        throw new NotImplementedException();
    }
}
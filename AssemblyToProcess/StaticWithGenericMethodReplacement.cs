using Ionad;

[StaticReplacement(typeof(StaticWithGenericMethod))]
public static class StaticWithGenericMethodReplacement
{
    public static T Method<T>()
    {
        return default;
    }
}
using Ionad;

[StaticReplacement(typeof(WithGenericMethodToSubstitute))]
public static class WithGenericMethodSubstitute
{
    public static T Method<T>()
    {
        return default(T);
    }
}
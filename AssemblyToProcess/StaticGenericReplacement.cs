using Ionad;

[StaticReplacement(typeof(StaticGeneric<>))]
public class StaticGenericReplacement<T>
{
    public static void SomeMethod()
    {
    }
}
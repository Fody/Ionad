using System;

public class WithGenericMethodToSubstitute
{
    public static T Method<T>()
    {
        throw new NotImplementedException();
    }
}
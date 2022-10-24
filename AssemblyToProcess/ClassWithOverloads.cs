public class ClassWithOverloads
{
    public int Overloaded0()
    {
        return StaticWithOverloads.Overloaded(0L);
    }

    public int Overloaded1()
    {
        return StaticWithOverloads.Overloaded();
    }

    public int Overloaded2()
    {
        return StaticWithOverloads.Overloaded(0);
    }

    public int Overloaded3()
    {
        return StaticWithOverloads.Overloaded("");
    }

    public Dictionary<int, int> Overloaded4()
    {
        return StaticWithOverloads.Overloaded<int, int>();
    }

    public Dictionary<int, string> Overloaded5()
    {
        return StaticWithOverloads.Overloaded(0, "Hello World");
    }
}
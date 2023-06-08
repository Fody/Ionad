using Ionad;

[StaticReplacement(typeof(StaticWithBaseAccess))]
public static class StaticWithBaseAccessReplacement
{
    public static int ReplacedCount
    {
        get
        {
            using(ThrowOnRecursion.Check())
            {
                return StaticWithBaseAccess.ReplacedCount + 1;
            }
        }
    }

    public static IEnumerable<int> YieldMethod()
    {
        using(ThrowOnRecursion.Check())
        {
            foreach (var i in StaticWithBaseAccess.YieldMethod())
            {
                yield return i + 10;
            }
        }
    }

    public static Task<int> LambdaReplacementMethod()
    {
        using(ThrowOnRecursion.Check())
        {
            return Task.Run(async () => await StaticWithBaseAccess.LambdaReplacementMethod() + 1);
        }
    }

    public static async Task<int> AsyncMethod()
    {
        await Task.Delay(1);
        return await StaticWithBaseAccess.AsyncMethod() + 1;
    }
}

public static class ThrowOnRecursion
{
    static AsyncLocal<int> CallCount = new();

    public static IDisposable Check()
    {
        if (CallCount.Value == 0)
        {
            CallCount.Value++;
            return new ExitRecursion();

        }

        throw new InvalidOperationException("Recursion detected");
    }

    class ExitRecursion :
        IDisposable
    {
        public void Dispose()
        {
            CallCount.Value--;
        }
    }
}
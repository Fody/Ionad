
using System.Collections.Generic;
using System.Threading.Tasks;

public static class StaticWithBaseAccess
{
    public static int ReplacedCount => 0;
    
    public static IEnumerable<int> YieldMethod()
    {
        for (var i = 0; i < 10; i++)
            yield return i;
    }

    public static Task<int> LambdaReplacementMethod()
    {
        return Task.FromResult(0);
    }

    public static async Task<int> AsyncMethod()
    {
        await Task.Delay(1);
        return 0;
    }
}

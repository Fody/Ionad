
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    private static AsyncLocal<int> CallCount = new AsyncLocal<int>();
    public static IDisposable Check()
    {
        if (CallCount.Value == 0)
        {
            CallCount.Value++;
            return new ExitRecursion();
            
        }
        else
        {
            throw new InvalidOperationException("Recursion detected");
        }
    }

    private class ExitRecursion : IDisposable
    {
        public void Dispose()
        {
            ThrowOnRecursion.CallCount.Value--;
        }
    }
}
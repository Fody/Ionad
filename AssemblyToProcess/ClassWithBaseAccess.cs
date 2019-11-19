using System.Collections.Generic;
using System.Threading.Tasks;

public class ClassWithBaseAccess
{
    public int ReplacedCount => StaticWithBaseAccess.ReplacedCount;
    public IEnumerable<int> Yield => StaticWithBaseAccess.YieldMethod();

    public Task<int> AsyncWithLambdaReplacement => StaticWithBaseAccess.LambdaReplacementMethod();

    public Task<int> AsyncDecorator => StaticWithBaseAccess.AsyncMethod();
}
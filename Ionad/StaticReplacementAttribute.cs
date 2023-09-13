namespace Ionad;

/// <summary>
/// Marks a class as a replacement for a static class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StaticReplacementAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticReplacementAttribute"/> with a System.Type.
    /// </summary>
    /// <param name="replacementType">The System.Type to be replaced with this class.</param>
    public StaticReplacementAttribute(Type replacementType)
    {
    }
}
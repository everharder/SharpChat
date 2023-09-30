namespace SharpChat.Functions;

/// <summary>
/// Service for creating <see cref="SharpFunction"/> instances
/// </summary>
internal interface ISharpFunctionFactory
{
    /// <summary>
    /// Create a <see cref="SharpFunction"/> from a method delegate
    /// Attention: The delegate must not be anonymous!
    /// TODO: write test for this
    /// </summary>
    SharpFunction CreateSharpFunction(Delegate func);
}
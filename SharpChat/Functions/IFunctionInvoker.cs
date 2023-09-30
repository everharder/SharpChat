using Azure.AI.OpenAI;
using SharpChat.Functions;

namespace SharpChat.Functions;

/// <summary>
/// Service for invoking registered functions
/// </summary>
public interface IFunctionInvoker
{
    /// <summary>
    /// Call a function defined by a <see cref="FunctionCall"/>
    /// </summary>
    /// <returns>The json serialized result of the function execution</returns>
    object? CallFunction(FunctionCall call);
}
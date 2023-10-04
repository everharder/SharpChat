namespace SharpChat;

/// <summary>
/// This attribute is used to signal that a <see cref="SharpFunctionParameter"/> should be unwrapped
/// A complex object will thus be processed by it's constructor parameters
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class UnwrapAttribute : Attribute
{
}

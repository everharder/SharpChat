using System.ComponentModel;

namespace SharpChat.Tests.Functions;

internal class Callbacks
{
    public int MethodWithoutDescription()
        => 0;

    [Description("A method without parameters")]
    public int MethodWithoutParams()
        => 0;

    [Description("A method with a single parameter")]
    public int MethodWithSingleParam([Description("A singular text parameter")] string text)
        => HashCode.Combine(text);

    [Description("A method with multiple parameters")]
    public int MethodWithMultipleParams(
        [Description("Random text parameter")] string text,
        [Description("Some integer")] int quantity,
        [Description("Useless boolean")] bool enabled,
        [Description("some optional string")] string optionalString = null!,
        [Description("Some float")] float number = 42f)
        => HashCode.Combine(text, quantity, enabled, optionalString, number);

    [Description("A method with a complex value")]
    public int MethodWithComplexValue([Unwrap] ComplexValue complexValue)
        => HashCode.Combine(complexValue.Id, complexValue.Name);

    [Description("A method with an enum")]
    public int MethodWithEnum([Description("Some enum value")] DummyEnum enumValue)
        => HashCode.Combine(enumValue);

    public record ComplexValue(
        [Description("A unique id property")] int Id,
        [Description("The name of something")] string? Name = "foobar");

    public enum DummyEnum
    {
        Foo = 0,
        Bar = 1,
        Baz = 2,
    }
}

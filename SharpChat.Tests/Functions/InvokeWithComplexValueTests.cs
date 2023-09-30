using System.Text.Json;
using Azure.AI.OpenAI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpChat.Functions;
using static SharpChat.Tests.Functions.Callbacks;

namespace SharpChat.Tests.Functions;

[TestClass]
public class InvokeWithComplexValueTests
{
    private static Callbacks callbacks = new Callbacks();

    [TestMethod]
    public void CallFunctionWithMultipleParameters_OptionalParametersPassed_Works()
    {
        FunctionService cot = new(new SharpFunctionFactory());
        cot.RegisterFunction(callbacks.MethodWithComplexValue);

        string payload = JsonSerializer.Serialize(new ComplexValue(420, "yeah"));
        object? result = cot.CallFunction(new FunctionCall(nameof(callbacks.MethodWithComplexValue), payload));

        int expected = HashCode.Combine(420, "yeah");
        result.Should().Be(expected);
    }

    [TestMethod]
    public void CallFunctionWithMultipleParameters_OnlyRequiredParametersPassed_Works()
    {
        FunctionService cot = new(new SharpFunctionFactory());
        cot.RegisterFunction(callbacks.MethodWithComplexValue);

        string payload = JsonSerializer.Serialize(new { Id = 420 });
        object? result = cot.CallFunction(new FunctionCall(nameof(callbacks.MethodWithComplexValue), payload));

        int expected = HashCode.Combine(420, "foobar");
        result.Should().Be(expected);
    }
}

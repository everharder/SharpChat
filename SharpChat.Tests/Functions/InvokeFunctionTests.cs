using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.AI.OpenAI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpChat.Functions;
using static SharpChat.Tests.Functions.Callbacks;

namespace SharpChat.Tests.Functions
{
    [TestClass]
    public class InvokeFunctionTests
    {
        private static Callbacks callbacks = new Callbacks();

        [TestMethod]
        public void FunctionNotRegistered_WhenCalled_Throws()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            FluentActions.Invoking(() => Call(cot, nameof(callbacks.MethodWithoutParams)))
                .Should()
                .Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void CallFunctionWithoutParameters_NoParametersPassed_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithoutParams));

            object result = Call(cot, nameof(callbacks.MethodWithoutParams));
            result.Should().Be(0);
        }

        [TestMethod]
        public void CallFunctionWithoutParameters_TooManyParametersPassed_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithoutParams));

            object result = Call(cot, nameof(callbacks.MethodWithoutParams), new Dictionary<string, object>()
        {
            { "ParameterDoesNotExist", "foobar" }
        });
            result.Should().Be(0);
        }

        [TestMethod]
        public void CallFunctionWithSingleParameter_ParameterPassed_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithSingleParam));

            object result = Call(cot, nameof(callbacks.MethodWithSingleParam), new Dictionary<string, object>()
            {
                { "text", "foobar" }
            });

            int expected = HashCode.Combine("foobar");
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithSingleParameter_AdditionalParameterPassed_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithSingleParam));

            object result = Call(cot, nameof(callbacks.MethodWithSingleParam), new Dictionary<string, object>()
        {
            { "AdiitionalParameter", "nonse" },
            { "text", "foobar" }
        });

            int expected = HashCode.Combine("foobar");
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithEnum_EnumMember_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithEnum));

            object result = Call(cot, nameof(callbacks.MethodWithEnum), new Dictionary<string, object>()
        {
            { "enumValue", "Baz" }
        });

            int expected = HashCode.Combine(DummyEnum.Baz);
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithEnum_InvalidEnumMember_ReturnsErrorString()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithEnum));

            string result = Call(cot, nameof(callbacks.MethodWithEnum), new Dictionary<string, object>()
            {
                { "enumValue", "Bang" }
            }) as string;

            result.Should().Be("Requested value 'Bang' was not found.");
        }

        [TestMethod]
        public void CallFunctionWithSingleParameter_TooFewParameterPassed_ReturnsErrorString()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithSingleParam));

            string result = Call(cot, nameof(callbacks.MethodWithSingleParam), new Dictionary<string, object>()) as string;
            result.Should().Be("Parameter 'text' is required!");
        }

        [TestMethod]
        public void CallFunctionWithMultipleParameters_OnlyRequiredParametersPassed_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithMultipleParams));

            object result = Call(cot, nameof(callbacks.MethodWithMultipleParams), new Dictionary<string, object>()
        {
            { "text", "foobar" },
            { "quantity", 42 },
            { "enabled", true },
        });

            int expected = HashCode.Combine("foobar", 42, true, (string)null, 42f);
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithMultipleParameters_OptionalParametersPassed_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithMultipleParams));

            object result = Call(cot, nameof(callbacks.MethodWithMultipleParams), new Dictionary<string, object>()
            {
                { "text", "foobar" },
                { "quantity", 42 },
                { "enabled", true },
                { "optionalString", "someValue" },
                { "number", 69f },
            });

            int expected = HashCode.Combine("foobar", 42, true, "someValue", 69f);
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithArrayParameter_FilledArray_Works()
        {
            FunctionService cot = new FunctionService(new SharpFunctionFactory());
            cot.RegisterFunction(callbacks, nameof(callbacks.MethodWithArray));

            var arrayValue = new int[] { 42, 69, 420 };

            object result = Call(cot, nameof(callbacks.MethodWithArray), new Dictionary<string, object>()
            {
                { "arrayValue", arrayValue },
            });

            int expected = string.Join(",", arrayValue).GetHashCode();
            result.Should().Be(expected);
        }

        private object Call(FunctionService service, string name, Dictionary<string, object> parameters = null)
        {
            string serialized = parameters != null
                ? SerializeArguments(parameters)
                : string.Empty;
            return service.CallFunction(new FunctionCall(name, serialized));
        }

        private string SerializeArguments(Dictionary<string, object> parameters)
            => JsonSerializer.Serialize(parameters);
    }
}
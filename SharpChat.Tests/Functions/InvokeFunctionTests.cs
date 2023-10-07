using System;
using System.Collections.Generic;
using System.Reflection;
using Azure.AI.OpenAI;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpChat.Functions;
using SharpChat.Utility;
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
            IServiceProvider services = Services
               .CreateServiceProvider();

            IFunctionInvoker invoker = services.GetRequiredService<IFunctionInvoker>();

            FluentActions.Invoking(() => invoker.CallFunction(new FunctionCall(nameof(callbacks.MethodWithoutParams), null)))
                .Should()
                .Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void CallFunctionWithoutParameters_NoParametersPassed_Works()
        {
            object result = Call(callbacks.MethodWithoutParams);
            result.Should().Be(0);
        }

        [TestMethod]
        public void CallFunctionWithoutParameters_TooManyParametersPassed_Works()
        {
            object result = Call(callbacks.MethodWithoutParams, new Dictionary<string, object>()
            {
                { "ParameterDoesNotExist", "foobar" }
            });
            result.Should().Be(0);
        }

        [TestMethod]
        public void CallFunctionWithSingleParameter_ParameterPassed_Works()
        {
            object result = Call(callbacks.MethodWithSingleParam, new Dictionary<string, object>()
            {
                { "text", "foobar" }
            });

            int expected = HashCode.Combine("foobar");
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithSingleParameter_AdditionalParameterPassed_Works()
        {
            object result = Call(callbacks.MethodWithSingleParam, new Dictionary<string, object>()
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
            object result = Call(callbacks.MethodWithEnum, new Dictionary<string, object>()
            {
                { "enumValue", "Baz" }
            });

            int expected = HashCode.Combine(DummyEnum.Baz);
            result.Should().Be(expected);
        }

        [TestMethod]
        public void CallFunctionWithEnum_InvalidEnumMember_ReturnsErrorString()
        {
            string result = Call(callbacks.MethodWithEnum, new Dictionary<string, object>()
            {
                { "enumValue", "Bang" }
            }) as string;

            result.Should().Be("Requested value 'Bang' was not found.");
        }

        [TestMethod]
        public void CallFunctionWithSingleParameter_TooFewParameterPassed_ReturnsErrorString()
        {
            string result = Call(callbacks.MethodWithSingleParam, new Dictionary<string, object>()) as string;
            result.Should().Be("Parameter 'text' is required, but was not provided!");
        }

        [TestMethod]
        public void CallFunctionWithMultipleParameters_OnlyRequiredParametersPassed_Works()
        {
            object result = Call(callbacks.MethodWithMultipleParams, new Dictionary<string, object>()
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
            object result = Call(callbacks.MethodWithMultipleParams, new Dictionary<string, object>()
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
            var arrayValue = new int[] { 42, 69, 420 };

            object result = Call(callbacks.MethodWithArray, new Dictionary<string, object>()
            {
                { "arrayValue", arrayValue },
            });

            int expected = string.Join(",", arrayValue).GetHashCode();
            result.Should().Be(expected);
        }

        private object Call(Delegate functionToCall, Dictionary<string, object> parameters = null)
        {
            IServiceProvider services = Services
                .CreateServiceProvider((r) => r.RegisterFunction(functionToCall));

            string serialized = parameters != null
                ? services.GetRequiredService<ISerializer>().Serialize(parameters)
                : string.Empty;

            IFunctionInvoker invoker = services.GetRequiredService<IFunctionInvoker>();

            return invoker.CallFunction(new FunctionCall(functionToCall.GetMethodInfo().Name, serialized));
        }
    }
}
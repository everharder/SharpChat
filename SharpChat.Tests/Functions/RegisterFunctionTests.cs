using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpChat.Functions;
using System;
using System.Linq;
using static SharpChat.Tests.Functions.Callbacks;

namespace SharpChat.Tests.Functions
{
    [TestClass]
    public class RegisterFunctionTests
    {
        private static Callbacks callbacks = new Callbacks();

        [TestMethod]
        public void RegisterFunction_NoDescription_Works()
        {
            var cot = new SharpFunctionFactory();
            SharpFunction function = cot.CreateSharpFunction(callbacks, nameof(callbacks.MethodWithoutDescription));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithoutDescription));
            function.Description.Should().BeNull();
            function.Parameters.Should().BeEmpty();
        }

        [TestMethod]
        public void RegisterFunction_NoParameters_Works()
        {
            var cot = new SharpFunctionFactory();
            SharpFunction function = cot.CreateSharpFunction(callbacks, nameof(callbacks.MethodWithoutParams));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithoutParams));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().BeEmpty();
        }

        [TestMethod]
        public void RegisterFunction_SingleParameter_Works()
        {
            var cot = new SharpFunctionFactory();
            SharpFunction function = cot.CreateSharpFunction(callbacks, nameof(callbacks.MethodWithSingleParam));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithSingleParam));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(1);


            SharpFunctionParameter parameter = function.Parameters.First();
            parameter.Name.Should().Be("text");
            parameter.DotNetType.Should().Be(typeof(string));
            parameter.JsType.Should().Be("string");
            parameter.IsRequired.Should().BeTrue();
            parameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
        }

        [TestMethod]
        public void RegisterFunction_Enum_Works()
        {
            var cot = new SharpFunctionFactory();
            SharpFunction function = cot.CreateSharpFunction(callbacks, nameof(callbacks.MethodWithEnum));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithEnum));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(1);

            SharpFunctionParameter parameter = function.Parameters.First();
            parameter.Name.Should().Be("enumValue");
            parameter.DotNetType.Should().Be(typeof(DummyEnum));
            parameter.JsType.Should().Be("string");
            parameter.IsRequired.Should().BeTrue();
            parameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
            parameter.EnumOptions.Should()
                .BeEquivalentTo(Enum.GetValues(typeof(DummyEnum)).Cast<DummyEnum>().Select(x => x.ToString()));
        }

        [TestMethod]
        public void RegisterFunction_MultipleParameters()
        {
            var cot = new SharpFunctionFactory();
            SharpFunction function = cot.CreateSharpFunction(callbacks, nameof(callbacks.MethodWithMultipleParams));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithMultipleParams));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(5);

            (string name, Type dotNetType, string jsType, bool isRequired, object defaultValue)[] expectedParameters =
            {
                ("text", typeof(string), "string", true, null),
                ("quantity", typeof(int), "number", true, null),
                ("enabled", typeof(bool), "boolean", true, null),
                ("optionalString", typeof(string), "string", false, null),
                ("number", typeof(float), "number", false, 42f),
            };

            int index = 0;
            foreach (var parameter in function.Parameters)
            {
                var (name, dotNetType, jsType, isRequired, defaultValue) = expectedParameters[index];
                parameter.Name.Should().Be(name);
                parameter.DotNetType.Should().Be(dotNetType);
                parameter.JsType.Should().Be(jsType);
                parameter.IsRequired.Should().Be(isRequired);
                parameter.Description.Should()
                    .NotBeNullOrWhiteSpace()
                    .And
                    .NotBe(function.Description);
                parameter.DefaultValue.Should().Be(defaultValue);

                index++;
            }
        }

        [TestMethod]
        public void RegisterFunction_WithComplexValue_DescriptionsUnwrapped()
        {
            var cot = new SharpFunctionFactory();
            SharpFunction function = cot.CreateSharpFunction(callbacks, nameof(callbacks.MethodWithComplexValue));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithComplexValue));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(2);

            function.Parameters.First().Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);

            function.Parameters.Skip(1).First().Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
        }
    }
}
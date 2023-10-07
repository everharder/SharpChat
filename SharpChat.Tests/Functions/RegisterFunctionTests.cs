using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpChat.Functions;
using SharpChat.Functions.Model;
using System;
using System.Linq;
using System.Reflection;
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
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();
            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithoutDescription));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithoutDescription));
            function.Description.Should().BeNull();
            function.Parameters.Should().BeEmpty();
        }

        [TestMethod]
        public void RegisterFunction_NoParameters_Works()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();

            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithoutParams));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithoutParams));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().BeEmpty();
        }

        [TestMethod]
        public void RegisterFunction_SingleParameter_Works()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();

            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithSingleParam));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithSingleParam));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(1);


            Property parameter = function.Parameters.First();
            parameter.Should().BeOfType<PrimitiveProperty>();
            parameter.Name.Should().Be("text");
            parameter.DotNetType.Should().Be(typeof(string));
            parameter.SchemaType.Should().Be("string");
            parameter.IsRequired.Should().BeTrue();
            parameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
        }

        [TestMethod]
        public void RegisterFunction_Enum_Works()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();

            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithEnum));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithEnum));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(1);

            Property parameter = function.Parameters.First();
            parameter.Should().BeOfType<EnumProperty>();
            parameter.Name.Should().Be("enumValue");
            parameter.DotNetType.Should().Be(typeof(DummyEnum));
            parameter.SchemaType.Should().Be("string");
            parameter.IsRequired.Should().BeTrue();
            parameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
        }

        [TestMethod]
        public void RegisterFunction_ArrayType_Works()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();

            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithArray));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithArray));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(1);

            Property parameter = function.Parameters.First();
            parameter.Should().BeOfType<ArrayProperty>();
            parameter.Name.Should().Be("arrayValue");
            parameter.DotNetType.Should().Be(typeof(int[]));
            parameter.SchemaType.Should().Be("array");
            parameter.IsRequired.Should().BeTrue();
            parameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);

            ArrayProperty arrParameter = parameter as ArrayProperty;
            arrParameter.Item.Should().BeOfType<PrimitiveProperty>();

            PrimitiveProperty elementParameter = arrParameter.Item as PrimitiveProperty;
            elementParameter.Name.Should().Be("arrayValue");
            elementParameter.DotNetType.Should().Be(typeof(int));
            elementParameter.SchemaType.Should().Be("number");
            elementParameter.IsRequired.Should().BeFalse();
            elementParameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
        }

        [TestMethod]
        public void RegisterFunction_ComplexArrayType_Works()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();

            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithComplexArray));

            function.Should().NotBeNull();
            function.Name.Should().Be(nameof(callbacks.MethodWithComplexArray));
            function.Description.Should().NotBeNullOrWhiteSpace();
            function.Parameters.Should().HaveCount(1);

            Property parameter = function.Parameters.First();
            parameter.Name.Should().Be("arrayValue");
            parameter.DotNetType.Should().Be(typeof(int[]));
            parameter.SchemaType.Should().Be("array");
            parameter.IsRequired.Should().BeTrue();
            parameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);

            ArrayProperty arrParameter = parameter as ArrayProperty;
            arrParameter.Item.Should().BeOfType<ObjectProperty>();

            ObjectProperty elementParameter = arrParameter.Item as ObjectProperty;
            elementParameter.Name.Should().Be("arrayValue");
            elementParameter.DotNetType.Should().Be(typeof(ComplexValue));
            elementParameter.SchemaType.Should().Be("object");
            elementParameter.IsRequired.Should().BeFalse();
            elementParameter.Description.Should()
                .NotBeNullOrWhiteSpace()
                .And
                .NotBe(function.Description);
        }

        [TestMethod]
        public void RegisterAllFunctions_WhenCalled_Works()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionRegistry cot = services.GetRequiredService<IFunctionRegistry>();

            cot.RegisterAllFunctions(callbacks);

            var registered = cot.GetRegisteredFunctions();

            var expected = callbacks
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && m.ReturnType != typeof(void))
                .ToList();

            registered.Count.Should().Be(expected.Count());
        }

        [TestMethod]
        public void RegisterFunction_MultipleParameters()
        {
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();
            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithMultipleParams));

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
                parameter.SchemaType.Should().Be(jsType);
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
            IServiceProvider services = Services.CreateServiceProvider();
            IFunctionFactory cot = services.GetRequiredService<IFunctionFactory>();
            Function function = cot.CreateFunction(callbacks, nameof(callbacks.MethodWithComplexValue));

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
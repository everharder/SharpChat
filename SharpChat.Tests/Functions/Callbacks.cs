using System;
using System.ComponentModel;
using System.Linq;

namespace SharpChat.Tests.Functions
{
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
            [Description("some optional string")] string optionalString = null,
            [Description("Some float")] float number = 42f)
            => HashCode.Combine(text, quantity, enabled, optionalString, number);

        [Description("A method with a complex value")]
        public int MethodWithComplexValue([Description("yeah complex value")] ComplexValue complexValue)
            => HashCode.Combine(complexValue.Id, complexValue.Name);

        [Description("A method with an enum")]
        public int MethodWithEnum([Description("Some enum value")] DummyEnum enumValue)
            => HashCode.Combine(enumValue);

        [Description("A method with an array parameter")]
        public int MethodWithArray([Description("Oh wow, an array")] int[] arrayValue)
            => string.Join(",", arrayValue).GetHashCode();

        [Description("A method with a complex array parameter")]
        public int MethodWithComplexArray([Description("Oh wow, an with a complex array")] ComplexValue[] arrayValue) 
            => arrayValue.Select(x => x.GetHashCode()).Sum();

        public class ComplexValue
        {
            public ComplexValue(
                [Description("A unique id property")] int Id,
                [Description("The name of something")] string Name = "foobar")
            {
                this.Id = Id;
                this.Name = Name;
            }

            public int Id { get; }
            public string Name { get; }
        }

        public enum DummyEnum
        {
            Foo = 0,
            Bar = 1,
            Baz = 2,
        }
    }
}
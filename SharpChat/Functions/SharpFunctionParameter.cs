using System;
using System.Linq;

namespace SharpChat.Functions
{
    /// <summary>
    /// A parameter for a <see cref="SharpFunction"/>
    /// </summary>
    /// <param name="Name">The parameter name</param>
    /// <param name="Description">Usage instructions for the chatbot</param>
    /// <param name="DotNetType">The parameter type in the function signature</param>
    /// <param name="IsRequired">Whether this parameter needs to present for function execution</param>
    /// <param name="DefaultValue">The default value of the parameter (only used if <paramref name="IsRequired"/> is false)</param>
    /// <param name="UnwrappedFromType">Set if this parameter has been synthesized from a complex type</param>
    internal class SharpFunctionParameter
    {
        /// <summary>
        /// The javascript type corresponding to the DotNetType
        /// Important for function schema definition that is exposed to the chatbot
        /// </summary>
        public string JsType => GetJsType(DotNetType);

        /// <summary>
        /// Whether this ´parameter was synthesized from a complex type
        /// </summary>
        public bool IsUnwrapped => UnwrappedFromType != null;

        /// <summary>
        /// If the parameter is an enum, we need to let the chatbot know the possible options
        /// </summary>
        public string[] EnumOptions => DotNetType.IsEnum
            ? Enum.GetValues(DotNetType).Cast<object>().Select(x => x.ToString()).ToArray()
            : null;

        public string Name { get; }
        public string Description { get; }
        public Type DotNetType { get; }
        public bool IsRequired { get; }
        public object DefaultValue { get; }
        public Type UnwrappedFromType { get; }

        public SharpFunctionParameter(
            string Name,
            string Description,
            Type DotNetType,
            bool IsRequired = true,
            object DefaultValue = null,
            Type UnwrappedFromType = null)
        {
            this.Name = Name;
            this.Description = Description;
            this.DotNetType = DotNetType;
            this.IsRequired = IsRequired;
            this.DefaultValue = DefaultValue;
            this.UnwrappedFromType = UnwrappedFromType;
        }

        private string GetJsType(Type dotNetType)
        {
            if (dotNetType.Equals(typeof(bool)))
            {
                return "boolean";
            }

            if (dotNetType.Equals(typeof(int))
                || dotNetType.Equals(typeof(short))
                || dotNetType.Equals(typeof(long))
                || dotNetType.Equals(typeof(float))
                || dotNetType.Equals(typeof(double)))
            {
                return "number";
            }

            // lets be explicit here
            if (dotNetType.Equals(typeof(string)) || dotNetType.IsEnum)
            {
                return "string";
            }

            throw new NotSupportedException($"{nameof(dotNetType)} is not supported!");
        }
    }
}
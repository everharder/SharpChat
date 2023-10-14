using SharpChat.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SharpChat.Functions.Model
{
    /// <summary>
    /// A property of primitve type
    /// </summary>
    public class PrimitiveProperty : Property
    {
        /// <inheritdoc/>
        public override string SchemaType => GetJsType(DotNetType);

        /// <summary>
        /// Create a new <see cref="PrimitiveProperty"/> instance
        /// </summary>
        public PrimitiveProperty(string name, 
            string description, 
            Type dotNetType,
            bool isRequired,
            object defaultValue) : base(name, description, dotNetType, isRequired, defaultValue)
        {
        }

        private string GetJsType(Type dotNetType)
        {
            string jsType;
            if (dotNetType.Equals(typeof(bool)))
            {
                jsType = "boolean";
            }
            else if (dotNetType.Equals(typeof(int))
                || dotNetType.Equals(typeof(short))
                || dotNetType.Equals(typeof(long))
                || dotNetType.Equals(typeof(float))
                || dotNetType.Equals(typeof(double)))
            {
                jsType = "number";
            }
            else if (dotNetType.Equals(typeof(string)))
            {
                jsType = "string";
            }
            else
            {
                throw new NotSupportedException($"Type '{dotNetType.Name}' is not supported!");
            }
            return jsType;
        }

        /// <inheritdoc/>
        public override object DeserializeValue(string data, ISerializer serializer) 
            => Convert.ChangeType(data, DotNetType, new CultureInfo("en"));
    }
}

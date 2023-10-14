using SharpChat.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpChat.Functions.Model
{
    /// <summary>
    /// A property of enum type
    /// </summary>
    public class EnumProperty : Property
    {
        /// <inheritdoc/>
        public override string SchemaType => "string";

        /// <summary>
        /// Creates a new instance of <see cref="EnumProperty"/>
        /// </summary>
        public EnumProperty(
            string name, 
            string description, 
            Type dotNetType,
            bool isRequired,
            object defaultValue) : base(name, description, dotNetType, isRequired, defaultValue)
        {
        }
        
        /// <inheritdoc/>
        public override Dictionary<string, object> GetParametersSchema()
        {
            Dictionary<string, object> schema = base.GetParametersSchema();
            schema["enum"] = Enum
                .GetValues(DotNetType)
                .Cast<object>()
                .Select(x => x.ToString())
                .ToArray();
            return schema;
        }

        /// <inheritdoc/>
        public override object DeserializeValue(string data, ISerializer serializer) 
            => Enum.Parse(DotNetType, data);
    }
}

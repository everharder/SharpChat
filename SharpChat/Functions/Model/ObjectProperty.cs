using SharpChat.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SharpChat.Functions.Model
{
    /// <summary>
    /// A complex object property
    /// </summary>
    public class ObjectProperty : Property
    {
        /// <summary>
        /// The object properties
        /// </summary>
        public Property[] Properties;

        /// <inheritdoc/>
        public override string SchemaType => "object";

        /// <summary>
        /// Creates a new <see cref="ObjectProperty"/> instance
        /// </summary>
        public ObjectProperty(
            string name, 
            string description, 
            Type dotNetType, 
            bool isRequired,
            object defaultValue,
            Property[] properties) : base(name, description, dotNetType, isRequired, defaultValue)
        {
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        }

        /// <inheritdoc/>
        public override Dictionary<string, object> GetParametersSchema()
        {
            Dictionary<string, object> schema = base.GetParametersSchema();
            schema["properties"] = Properties.ToDictionary(k => k.Name, v => v.GetParametersSchema());
            return schema;
        }

        /// <inheritdoc/>
        public override object DeserializeValue(string data, ISerializer serializer) 
            => serializer.Deserialize(data, DotNetType);
    }
}

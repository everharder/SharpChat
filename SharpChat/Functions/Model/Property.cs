using SharpChat.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpChat.Functions.Model
{
    /// <summary>
    /// Property base class
    /// </summary>
    public abstract class Property : ISchemaProvider
    {
        /// <summary>
        /// The property name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type name in the property schema
        /// </summary>
        public abstract string SchemaType { get; }

        /// <summary>
        /// The property description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The dotnet type of the property
        /// </summary>
        public Type DotNetType { get; }

        /// <summary>
        /// Whether to property is required to call the function
        /// If it is required and missing, an error is thrown
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// The default value for the property
        /// Properties with default values are automatically not required
        /// </summary>
        public object DefaultValue { get; }

        public Property(string name, string description, Type dotNetType, bool isRequired, object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Description = description;
            DotNetType = dotNetType ?? throw new ArgumentNullException(nameof(dotNetType));
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }

        /// <inheritdoc/>
        public virtual Dictionary<string, object> GetSchema() 
            => new Dictionary<string, object>()
            {
                { "type", SchemaType },
                { "description", Description }
            };

        /// <summary>
        /// Deserialize a value of the property type
        /// </summary>
        public abstract object DeserializeValue(string data, ISerializer serializer);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace SharpChat.Functions.Model
{
    /// <summary>
    /// A function definition for the chatbot to use
    /// </summary>
    public class Function : ISchemaProvider
    {
        /// <summary>
        /// Name of the function
        /// </summary>
        public string Name => MethodInfo.Name;

        /// <summary>
        /// The description (<seealso cref="DescriptionAttribute"/>)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The instance the function is invoked on
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// The method info of the function
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// The parameters of the function
        /// </summary>
        public Property[] Parameters { get; set; }

        /// <summary>
        /// Creates a new <see cref="Function"/> instance
        /// </summary>
        public Function(MethodInfo methodInfo, object target, string description, Property[] parameters)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Description = description;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <inheritdoc/>
        public Dictionary<string, object> GetParametersSchema()
        {
            Dictionary<string, object> schema = new Dictionary<string, object>();
            schema["type"] = "object";
            schema["properties"] = Parameters.ToDictionary(k => k.Name, v => v.GetParametersSchema());
            return schema;
        }
    }
}

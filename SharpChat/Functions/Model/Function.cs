using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpChat.Functions.Model
{
    public class Function : ISchemaProvider
    {
        public string Name => MethodInfo.Name;
        public string Description { get; set; }
        public object Target { get; }
        public MethodInfo MethodInfo { get; }
        public Property[] Parameters { get; set; }

        public Function(MethodInfo methodInfo, object target, string description, Property[] parameters)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Description = description;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <inheritdoc/>
        public Dictionary<string, object> GetSchema()
        {
            Dictionary<string, object> schema = new Dictionary<string, object>()
            {
                { "name", Name },
                { "description", Description },
            };
            if(Parameters.Any())
            {
                schema["parameters"] = new Dictionary<string, object>()
                {
                    {"type", "object" },
                    {"properties", Parameters.ToDictionary(k => k.Name, v => v.GetSchema()) }
                };
            }
            return schema;
        }
    }
}

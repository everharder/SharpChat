using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace SharpChat.Functions
{
    /// <summary>
    /// Holds the definition of a function that the chatbot can call
    /// </summary>
    /// <param name="Name">The function name</param>
    /// <param name="Delegate">The delegate to invoke by the chatbot</param>
    /// <param name="Description">The description that is exposed to the chatbot (should be informative!)</param>
    /// <param name="Parameters">The function parameters</param>
    internal class SharpFunction
    {
        private readonly object target;
        private readonly MethodInfo methodInfo;

        /// <summary>
        /// Whether the function parameters are part of a complex object
        /// </summary>
        public bool HasUnwrappedParameters
            => Parameters.Any(x => x.IsUnwrapped);

        /// <summary>
        /// The type of the parameter that was unwrapped (if <see cref="HasUnwrappedParameters"/> is true)
        /// </summary>
        public Type UnwrappedParameterType
            => Parameters.FirstOrDefault(x => x.IsUnwrapped)?.UnwrappedFromType;

        public string Name => methodInfo.Name;
        public string Description { get; }
        public IReadOnlyCollection<SharpFunctionParameter> Parameters { get; }

        public SharpFunction(
            object target,
            MethodInfo methodInfo,
            string description,
            IReadOnlyCollection<SharpFunctionParameter> parameters)
        {
            this.target = target ?? throw new ArgumentNullException(nameof(target));
            this.methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            this.Description = description;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Get a JSON serialized definition of the parameters as OpenAI expects it
        /// </summary>
        public string GetParameterDefinitionJson()
        {
            Dictionary<string, Dictionary<string, object>> parameterDefinition = Parameters
                .ToDictionary(k => k.Name, v => GetParameterDefinitionJson(v));

            Dictionary<string, object> definition = new Dictionary<string, object>()
            {
                { "type", "object" },
                { "properties", parameterDefinition },
                { "required", Parameters.Where(x => x.IsRequired).Select(x => x.Name).ToArray() }
            };

            return JsonSerializer.Serialize(definition);
        }

        /// <summary>
        /// Invoke the bound function
        /// </summary>
        public object Invoke(object[] parameters) 
            => this.methodInfo.Invoke(this.target, parameters);

        private Dictionary<string, object> GetParameterDefinitionJson(SharpFunctionParameter parameter)
        {
            Dictionary<string, object> definition = new Dictionary<string, object>()
            {
                {"type", parameter.JsType }
            };
            if(parameter.IsArray)
            {
                definition["items"] = new Dictionary<string, object>()
                {
                    {"type", parameter.ElementJsType }
                };
            }

            if (!string.IsNullOrWhiteSpace(parameter.Description))
            {
                definition["description"] = parameter.Description;
            }

            if (parameter.EnumOptions?.Length > 0)
            {
                definition["enum"] = parameter.EnumOptions;
            }

            return definition;
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;

namespace SharpChat.Functions
{
    /// <summary>
    /// A combined <see cref="IFunctionRegistry"/> and <see cref="IFunctionInvoker"/>
    /// </summary>
    internal class FunctionService : IFunctionInvoker, IFunctionRegistry
    {
        #region fields

        private ConcurrentDictionary<string, SharpFunction> registrations = new ConcurrentDictionary<string, SharpFunction>(StringComparer.OrdinalIgnoreCase);
        private readonly ISharpFunctionFactory factory;

        #endregion


        #region ctor

        public FunctionService(ISharpFunctionFactory factory)
        {
            this.factory = factory;
        }

        #endregion


        #region IFunctionInvoker methods

        /// <inheritdoc/>
        public object CallFunction(FunctionCall call)
        {
            // get the registered meta function
            SharpFunction function = GetFunction(call);

            try
            {
                // parse the function arguments from the chatbot
                object[] orderedParameters = null;
                if (!string.IsNullOrWhiteSpace(call.Arguments))
                {
                    // if the function is defined by a complex (unwrapped) object, we need to reconstruct it
                    if (function.HasUnwrappedParameters)
                    {
                        // deserialize object JSON
                        object unwrappedObj = JsonSerializer.Deserialize(
                            json: call.Arguments,
                            returnType: function.UnwrappedParameterType,
                            options: GetJsonOptions());

                        if (unwrappedObj == null)
                        {
                            throw new ArgumentException($"Failed to deserialize args {call.Arguments} into unwrapped type {function.UnwrappedParameterType}");
                        }

                        orderedParameters = new object[] { unwrappedObj };
                    }
                    else
                    {
                        Dictionary<string, string> parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            json: call.Arguments,
                            options: GetJsonOptions())
                            ?.ToDictionary(k => k.Key, v => v.Value.ToString(), StringComparer.OrdinalIgnoreCase)
                            ?? new Dictionary<string, string>();

                        // ensure correct ordering and type of parameters
                        orderedParameters = function.Parameters
                            .Select(p => CastParameter(p, parameters.ContainsKey(p.Name) ? parameters[p.Name] : null))
                            .ToArray();
                    }
                }

                return function.Invoke(orderedParameters);
            }
            catch (Exception ex)
            {
                // return erros as function results and let the LLM handle correcting it
                return ex.Message;
            }
        }

        #endregion


        #region IFunctionRegistry methods

        /// <inheritdoc/>
        public IFunctionRegistry RegisterFunction(Delegate function) 
            => RegisterFunction(function?.Target, function?.Method?.Name);

        /// <inheritdoc/>
        public IFunctionRegistry RegisterFunction(object target, string name)
        {
            SharpFunction function = factory.CreateSharpFunction(target, name);
            string normalizedName = NormalizeFunctionName(function.Name);
            if (!registrations.TryAdd(normalizedName, function))
            {
                throw new ArgumentException($"Function with name {normalizedName} is already registered");
            }
            return this;
        }

        /// <inheritdoc/>
        public IFunctionRegistry RegisterAllFunctions(object target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // Retrieve all functions from the object
            List<MethodInfo> functions = target
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && m.ReturnType != typeof(void))
                .ToList();

            // Convert functions to delegates and register them
            foreach (MethodInfo function in functions)
            {
                RegisterFunction(target, function.Name);
            }
            return this;
        }

        /// <inheritdoc/>
        public IList<FunctionDefinition> GetRegisteredFunctions()
        {
            return registrations.Values.Select(x => new FunctionDefinition()
            {
                Name = x.Name,
                Description = x.Description,
                Parameters = BinaryData.FromString(x.GetParameterDefinitionJson())
            }).ToList();
        }

        #endregion


        #region methods

        private object CastParameter(SharpFunctionParameter parameter, string valueAsString)
        {
            if (string.IsNullOrWhiteSpace(valueAsString))
            {
                if (parameter.IsRequired)
                {
                    throw new InvalidOperationException($"Parameter '{parameter.Name}' is required!");
                }
                return parameter.DefaultValue;
            }

            if (parameter.DotNetType.IsEnum)
            {
                return Enum.Parse(parameter.DotNetType, valueAsString);
            }

            return Convert.ChangeType(valueAsString, parameter.DotNetType, new CultureInfo("en"));
        }

        private string NormalizeFunctionName(string functionName)
            => functionName.Split('.').Last().ToLower().Trim();

        private SharpFunction GetFunction(FunctionCall call)
        {
            string name = NormalizeFunctionName(call.Name);
            if (!registrations.TryGetValue(name, out SharpFunction registration))
            {
                throw new InvalidOperationException($"Function with name {name} is not registered!");
            }
            return registration;
        }

        private JsonSerializerOptions GetJsonOptions()
            => new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

        #endregion
    }
}
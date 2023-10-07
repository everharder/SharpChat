using System;
using System.Collections.Generic;
using System.Linq;
using Azure.AI.OpenAI;
using SharpChat.Functions.Model;
using SharpChat.Utility;

namespace SharpChat.Functions
{
    /// <summary>
    /// A combined <see cref="IFunctionRegistry"/> and <see cref="IFunctionInvoker"/>
    /// </summary>
    internal class FunctionInvoker : IFunctionInvoker
    {
        #region fields

        private readonly IFunctionRegistryInternal registry;
        private readonly ISerializer serializer;

        #endregion


        #region lifecycle

        public FunctionInvoker(IFunctionRegistryInternal registry, ISerializer serializer)
        {
            this.registry = registry;
            this.serializer = serializer;
        }

        #endregion


        #region IFunctionInvoker methods

        /// <inheritdoc/>
        public object CallFunction(FunctionCall call)
        {
            // get the registered meta function
            Function function = registry.GetFunction(call);

            try
            {
                // parse the function arguments from the chatbot
                List<object> orderedParameters = new List<object>();
                if (!string.IsNullOrWhiteSpace(call.Arguments))
                {
                    Dictionary<string, string> parameters = serializer.Deserialize<Dictionary<string, string>>(call.Arguments)
                        ?.ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase)
                        ?? new Dictionary<string, string>();

                    foreach(Property parameter in function.Parameters)
                    {
                        object parameterValue = null;
                        if (parameters.TryGetValue(parameter.Name, out string stringValue) && !string.IsNullOrEmpty(stringValue))
                        {
                            parameterValue = parameter.DeserializeValue(stringValue, serializer);
                        }

                        if(parameterValue == null && parameter.IsRequired)
                        {
                            throw new Exception($"parameter {parameter.Name} is required, but was not provided");
                        }

                        orderedParameters.Add(parameterValue ?? parameter.DefaultValue);
                    }
                }
                return function.MethodInfo.Invoke(function.Target, orderedParameters.ToArray());
            }
            catch (Exception ex)
            {
                // return erros as function results and let the LLM handle correcting it
                return ex.Message;
            }
        }

        #endregion
    }
}
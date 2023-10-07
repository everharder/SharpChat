using Azure.AI.OpenAI;
using SharpChat.Functions.Model;
using SharpChat.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpChat.Functions
{
    internal class FunctionRegistry : IFunctionRegistryInternal
    {
        #region fields

        private ConcurrentDictionary<string, Function> registrations = new ConcurrentDictionary<string, Function>(StringComparer.OrdinalIgnoreCase);
        private readonly IFunctionFactory factory;
        private readonly ISerializer serializer;

        #endregion


        #region lifecycle

        public FunctionRegistry(IFunctionFactory factory, ISerializer serializer)
        {
            this.factory = factory;
            this.serializer = serializer;
        }

        #endregion


        #region IFunctionRegistry methods

        /// <inheritdoc/>
        public IFunctionRegistry RegisterFunction(Delegate function)
            => RegisterFunction(function?.Target, function?.Method?.Name);

        /// <inheritdoc/>
        public IFunctionRegistry RegisterFunction(object target, string name)
        {
            Function function = factory.CreateFunction(target, name);
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
                Parameters = BinaryData.FromString(serializer.Serialize(x.GetSchema()))
            }).ToList();
        }

        private string NormalizeFunctionName(string functionName)
            => functionName.Split('.').Last().ToLower().Trim();

        public Function GetFunction(FunctionCall call)
        {
            string name = NormalizeFunctionName(call.Name);
            if (!registrations.TryGetValue(name, out Function registration))
            {
                throw new InvalidOperationException($"Function with name {name} is not registered!");
            }
            return registration;
        }

        #endregion
    }
}

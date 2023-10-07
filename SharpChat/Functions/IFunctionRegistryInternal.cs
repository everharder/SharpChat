using Azure.AI.OpenAI;
using SharpChat.Functions.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpChat.Functions
{
    internal interface IFunctionRegistryInternal : IFunctionRegistry
    {
        /// <summary>
        /// Get the function matching the function call
        /// </summary>
        Function GetFunction(FunctionCall call);
    }
}

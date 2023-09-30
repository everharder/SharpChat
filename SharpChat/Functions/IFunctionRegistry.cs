using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.OpenAI;

namespace SharpChat.Functions;

/// <summary>
/// A service for exposing functions to the chatbot
/// </summary>
public interface IFunctionRegistry
{
    /// <summary>
    /// Enable the chatbot to execute a given function
    /// </summary>
    IFunctionRegistry RegisterFunction(Delegate func);

    /// <summary>
    /// Get registered functions as <see cref="FunctionDefinition"/>s which can be passed to OpenAI API
    /// </summary>
    /// <returns></returns>
    IList<FunctionDefinition> GetRegisteredFunctions();
}

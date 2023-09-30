using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharpChat.Functions;

/// <summary>
/// Holds the definition of a function that the chatbot can call
/// </summary>
/// <param name="Name">The function name</param>
/// <param name="Delegate">The delegate to invoke by the chatbot</param>
/// <param name="Description">The description that is exposed to the chatbot (should be informative!)</param>
/// <param name="Parameters">The function parameters</param>
internal record SharpFunction(
    string Name,
    Delegate Delegate,
    string? Description,
    IReadOnlyCollection<SharpFunctionParameter> Parameters)
{
    /// <summary>
    /// Whether the function parameters are part of a complex object
    /// </summary>
    public bool HasUnwrappedParameters
        => Parameters.Any(x => x.IsUnwrapped);

    /// <summary>
    /// The type of the parameter that was unwrapped (if <see cref="HasUnwrappedParameters"/> is true)
    /// </summary>
    public Type? UnwrappedParameterType
        => Parameters.FirstOrDefault(x => x.IsUnwrapped)?.UnwrappedFromType;

    /// <summary>
    /// Get a JSON serialized definition of the parameters as OpenAI expects it
    /// </summary>
    public string GetParameterDefinitionJson()
    {
        Dictionary<string, Dictionary<string, object?>> parameterDefinition = Parameters
            .ToDictionary(k => k.Name, v => GetParameterDefinitionJson(v));

        Dictionary<string, object> definition = new()
            {
                { "type", "object" },
                { "properties", parameterDefinition },
                { "required", Parameters.Where(x => x.IsRequired).Select(x => x.Name).ToArray() }
            };

        return JsonSerializer.Serialize(definition);
    }

    private Dictionary<string, object?> GetParameterDefinitionJson(SharpFunctionParameter parameter)
    {
        Dictionary<string, object?> definition = new()
        {
            {"type", parameter.JsType }
        };

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

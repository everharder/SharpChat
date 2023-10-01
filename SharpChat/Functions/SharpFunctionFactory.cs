using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpChat.Functions;

/// <inheritdoc/>
internal class SharpFunctionFactory : ISharpFunctionFactory
{
    /// <inheritdoc/>
    public SharpFunction CreateSharpFunction(Delegate func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        MethodInfo methodInfo = func.Method;
        SharpFunctionParameter[] parameters = CreateSharpFunctionParameters(methodInfo);

        string? description = methodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
        SharpFunction functionInfo = new(
            Name: methodInfo.Name,
            Delegate: func,
            Description: description,
            Parameters: parameters);

        return functionInfo;
    }

    private SharpFunctionParameter[] CreateSharpFunctionParameters(MethodInfo methodInfo)
    {
        ParameterInfo[] parameters = methodInfo.GetParameters();
        Type? unwrappedFromType = null;

        if (parameters.Any(p => p.GetCustomAttribute<UnwrapAttribute>() != null))
        {
            if (parameters.Length > 1)
            {
                throw new NotSupportedException($"{nameof(UnwrapAttribute)} is currently only supported for single-parameter methods!");
            }

            unwrappedFromType = parameters[0].ParameterType;

            ConstructorInfo[] constructors = unwrappedFromType.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Type {unwrappedFromType.Name} must have exactly one constructor in order to be unwrappable");
            }

            if (!constructors[0].GetParameters().Any())
            {
                throw new InvalidOperationException($"Type to unwwrap {unwrappedFromType.Name} contains no constructor parameters");
            }

            parameters = constructors[0].GetParameters();
        }

        return parameters
            .Select(x => CreateSharpFunctionParameter(x, unwrappedFromType))
            .ToArray();
    }

    private SharpFunctionParameter CreateSharpFunctionParameter(ParameterInfo parameterInfo, Type? unwrappedType = null)
    {
        Type type = parameterInfo.ParameterType;
        if (type != typeof(string) && !type.IsPrimitive && !type.IsEnum)
        {
            throw new NotSupportedException($"Complex type {type.Name} of member {parameterInfo.Name} is not supported!");
        }

        string? description = parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
        return new SharpFunctionParameter(
            Name: parameterInfo.Name!,
            Description: description,
            DotNetType: parameterInfo.ParameterType,
            DefaultValue: parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : null,
            IsRequired: !parameterInfo.HasDefaultValue,
            UnwrappedFromType: unwrappedType);
    }
}

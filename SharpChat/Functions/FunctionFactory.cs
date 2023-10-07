using SharpChat.Functions.Model;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace SharpChat.Functions
{
    /// <inheritdoc/>
    internal class FunctionFactory : IFunctionFactory
    {
        /// <inheritdoc/>
        public Function CreateFunction(object target, string name)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            MethodInfo methodInfo = target
                .GetType()
                .GetMethod(name, BindingFlags.Instance | BindingFlags.Public);

            Property[] parameters = methodInfo
                .GetParameters()
                .Select(parameter => CreateProperty(parameter))
                .ToArray();

            string description = methodInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            return new Function(
                methodInfo: methodInfo,
                target: target,
                description: description,
                parameters: parameters);
        }

        private Property CreateProperty(ParameterInfo parameterInfo)
        {
            string description = parameterInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            return CreateProperty(
                name: parameterInfo.Name,
                dotNetType: parameterInfo.ParameterType,
                description: description,
                isRequired: !parameterInfo.HasDefaultValue,
                defaultValue: parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : null);
        }

        private Property CreateProperty(PropertyInfo propertyInfo)
        {
            string description = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            return CreateProperty(
                name: propertyInfo.Name,
                dotNetType: propertyInfo.PropertyType,
                description: description,
                isRequired: false,
                defaultValue: null);
        }

        private Property CreateProperty(string name, Type dotNetType, string description, bool isRequired, object defaultValue)
        {
            if(dotNetType.IsArray)
            {
                return new ArrayProperty(
                    name: name, 
                    description: description, 
                    dotNetType: dotNetType,
                    isRequired: isRequired,
                    defaultValue: defaultValue,
                    item: CreateProperty(name, dotNetType.GetElementType(), description, isRequired, defaultValue));
            }

            if(dotNetType.IsEnum)
            {
                return new EnumProperty(
                    name: name,
                    description: description,
                    dotNetType: dotNetType,
                    isRequired: isRequired,
                    defaultValue: defaultValue);
            }

            if (dotNetType != typeof(string) || dotNetType.IsPrimitive)
            {
                return new PrimitiveProperty(
                    name: name,
                    description: description,
                    dotNetType: dotNetType,
                    isRequired: isRequired,
                    defaultValue: defaultValue);
            }

            Property[] objectProperties = dotNetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(property => CreateProperty(property))
                .ToArray();

            return new ObjectProperty(
                name: name,
                description: description,
                dotNetType: dotNetType,
                isRequired: isRequired,
                defaultValue: defaultValue,
                properties: objectProperties);
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using SharpChat.Functions;
using System;

namespace SharpChat.Tests
{
    internal static class Services
    {
        public static IServiceProvider CreateServiceProvider()
            => CreateServiceProvider((Action<IFunctionRegistry>)null);

        public static IServiceProvider CreateServiceProvider(Action<IFunctionRegistry> functionRegistryAction)
            => CreateServiceProvider((r, _) => functionRegistryAction?.Invoke(r));

        public static IServiceProvider CreateServiceProvider(Action<IFunctionRegistry, IServiceProvider> functionRegistryAction)
            => new ServiceCollection().AddSharpChat(functionRegistryAction).BuildServiceProvider();
    }
}

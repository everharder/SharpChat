using Microsoft.Extensions.DependencyInjection;
using SharpChat.Chatting;
using SharpChat.Functions;
using SharpChat.Utility;
using System;

namespace SharpChat
{
    /// <summary>
    /// Service registration extensions
    /// </summary>
    public static class Services
    {
        /// <summary>
        /// Add sharpchat functionality to your service container.
        /// e.g.: var services = new ServiceCollection()
        ///             .AddSharpChat((f, _) => f
        ///             .RegisterFunction(shop.GetPrice)
        ///             .RegisterFunction(shop.PlaceOrder))
        ///             .BuildServiceProvider();
        /// <param name="services">Your service collection</param>
        /// <param name="functionRegistryAction">An action to register functions</param>
        /// <returns></returns>
        /// </summary>
        public static IServiceCollection AddSharpChat(this IServiceCollection services, Action<IFunctionRegistry, IServiceProvider> functionRegistryAction = null)
            => services
                .AddSingleton<IFunctionInvoker, FunctionInvoker>()
                .AddSingleton(c =>
                {
                    FunctionRegistry registry = new FunctionRegistry(c.GetRequiredService<IFunctionFactory>(), c.GetRequiredService<ISerializer>());
                    functionRegistryAction?.Invoke(registry, c);
                    return registry;
                })
                .AddSingleton<IFunctionRegistry>(s => s.GetRequiredService<FunctionRegistry>())
                .AddSingleton<IFunctionRegistryInternal>(s => s.GetRequiredService<FunctionRegistry>())
                .AddSingleton<IConversationFactory, ConversationFactory>()
                .AddSingleton<ISerializer, JsonSerializer>()
                .AddSingleton<IFunctionFactory, FunctionFactory>();  
    }
}
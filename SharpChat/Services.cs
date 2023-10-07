using Microsoft.Extensions.DependencyInjection;
using SharpChat.Chatting;
using SharpChat.Functions;
using SharpChat.Utility;
using System;

namespace SharpChat
{
    public static class Services
    {
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
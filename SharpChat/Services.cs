using Microsoft.Extensions.DependencyInjection;
using SharpChat.Chatting;
using SharpChat.Functions;

namespace SharpChat;

public static class Services
{
    public static IServiceCollection AddSharpChat(this IServiceCollection services, Action<IFunctionRegistry, IServiceProvider>? functionRegistryAction = null)
        => services
            .AddSingleton<FunctionService>()
            .AddSingleton<IFunctionInvoker>(s => s.GetRequiredService<FunctionService>())
            .AddSingleton<IFunctionRegistry>(s =>
            {
                FunctionService functionService = s.GetRequiredService<FunctionService>();
                functionRegistryAction?.Invoke(functionService, s);
                return functionService;
            })
            .AddSingleton<IConversationFactory, ConversationFactory>()
            .AddSingleton<ISharpFunctionFactory, SharpFunctionFactory>();
}

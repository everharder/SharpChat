using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using SharpChat;
using SharpChat.Chatting;

internal class Program
{
    private static async Task Main(string openAiApiKey, string modelName)
    {
        // setup sharpchat
        // expose functions to the chatbot
        var services = new ServiceCollection()
            .AddSharpChat((f,_) => f
                .RegisterFunction(Add)
                .RegisterFunction(Sub)
                .RegisterFunction(Mul)
                .RegisterFunction(Div))
            .BuildServiceProvider();

        // create conversation
        var factory = services.GetRequiredService<IConversationFactory>();
        var client = new OpenAIClient(openAiApiKey);
        var conversation = factory.StartConversation(client, modelName);

        // start prompting!
        var input = "What is 42*PI?";
        Console.WriteLine($"[user] {input}");
        var output = await conversation.Prompt(input);
        Console.WriteLine($"[chat] {output}");
    }


    public static float Add(float f1, float f2)
        => f1 + f2;
    public static float Sub(float f1, float f2)
        => f1 - f2;
    public static float Mul(float f1, float f2) 
        => f1 * f2;
    public static float Div(float f1, float f2)
        => f1 / f2;
}
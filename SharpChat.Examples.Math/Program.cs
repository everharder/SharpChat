using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using SharpChat;
using SharpChat.Chatting;
using System.Diagnostics.CodeAnalysis;

internal class Program
{
    /// <summary>
    /// SharpChat example illustrating how to register functions for the chatbot to use
    /// This greatly improves upon chatbot accuracy, eliminating hallucinations
    /// </summary>
    static async Task Main(string apikey, string model)
    {
        MyMath math = new MyMath();

        // setup sharpchat
        // expose functions to the chatbot
        var services = new ServiceCollection()
            .AddSharpChat((f,_) => f
                .RegisterFunction(math.Add)
                .RegisterFunction(math.Sub)
                .RegisterFunction(math.Mul)
                .RegisterFunction(math.Div))
            .BuildServiceProvider();

        // create conversation
        var factory = services.GetRequiredService<IConversationFactory>();
        var client = new OpenAIClient(apikey);
        var conversation = factory.StartConversation(client, model);

        // start prompting
        var input = "What is 42*PI?";
        Console.WriteLine($"[user] {input}");
        var output = await conversation.Prompt(input);
        Console.WriteLine($"[chat] {output}");
    }

    public class MyMath
    {
        public float Add(float f1, float f2)
        {
            Console.WriteLine($"[function] {nameof(Add)}({f1}, {f2})");
            return f1 + f2;
        }

        public float Sub(float f1, float f2)
        {
            Console.WriteLine($"[function] {nameof(Sub)}({f1}, {f2})");
            return f1 - f2;
        }

        public float Mul(float f1, float f2)
        {
            Console.WriteLine($"[function] {nameof(Mul)}({f1}, {f2})");
            return f1 * f2;
        }

        public float Div(float f1, float f2)
        {
            Console.WriteLine($"[function] {nameof(Div)}({f1}, {f2})");
            return f1 / f2;
        }
    }

}
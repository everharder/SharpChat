using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using SharpChat.Chatting;
using SharpChat;
using System.ComponentModel;
using System.Text;

internal class Program
{
    /// <summary>
    /// SharpChat example illustrating how to register functions for the chatbot to use
    /// This greatly improves upon chatbot accuracy, eliminating hallucinations
    /// </summary>
    static async Task Main(string apikey, string model)
    {
        var shop = new TeaShop();

        // setup sharpchat
        // expose functions to the chatbot
        var services = new ServiceCollection()
            .AddSharpChat((f, _) => f
                .RegisterFunction(shop.GetPrice)
                .RegisterFunction(shop.PlaceOrder))
            .BuildServiceProvider();

        // create conversation
        var factory = services.GetRequiredService<IConversationFactory>();
        var client = new OpenAIClient(apikey);
        var conversation = factory.StartConversation(client, model);

        // start prompting
        var input = "Hi, please pack me up some nice matcha for 10€ and place the order.";
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine($"[user] {input}");
        var output = await conversation.Prompt(input);
        Console.WriteLine($"[chat] {output}");
    }

    public enum TeaType
    {
        Green,
        Black,
        Fruit,
        Matcha,
        Herbal,
    }

    public class TeaShop
    {
        private Dictionary<TeaType, float> pricePer100g = new Dictionary<TeaType, float>()
        {
            { TeaType.Green, 6.4f },
            { TeaType.Black, 4.3f },
            { TeaType.Fruit, 6.4f },
            { TeaType.Matcha, 8.2f },
            { TeaType.Herbal, 4.0f },
        };

        [Description("Gets the EUR price of 100g of a certain tea type.")]
        public float GetPrice(TeaType teaType)
        {
            Console.WriteLine($"[function] GetPrice({teaType})");
            return pricePer100g[teaType];
        }

        [Description("Commit the purchase of some tea. Returns true if the order was successfully placed.")]
        public bool PlaceOrder(Order order)
        {
            Console.WriteLine($"[function] PlaceOrder({order.TeaType}, {order.Weight}, {order.Amount})");
            return true;
        }

        [Description("A customer order of tea")]
        public record Order([Description("The type of tea")] TeaType TeaType,
            [Description("The amount of tea to buy in grams")] float Weight,
            [Description("The total price of the purchase")] float Amount)
        {
        }
    }
}
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
        var input = "Hi, please order me matcha for 10€ and black tea for 20€. Immediately place the order.";
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

        [Description("Gets the EUR price of 100 grams of a certain tea type.")]
        public float GetPrice(TeaType teaType)
        {
            var price = pricePer100g[teaType];
            Console.WriteLine($"[function] GetPrice({teaType}) = {price}");
            return price;
        }

        [Description("Commit the purchase of some tea. Returns true if the order was successfully placed.")]
        public bool PlaceOrder(Order[] orders)
        {
            foreach (Order order in orders)
            {
                Console.WriteLine($"[function] PlaceOrder({order.TeaType}, {order.Grams}, {order.Price})");
            }
            return true;
        }

        [Description("A customer order of tea")]
        public record Order([Description("The type of tea")] TeaType TeaType,
            [Description("The weight (quantity) of the tea type in grams")] float Grams,
            [Description("How much is payed for the tea by the customer")] float Price)
        {
        }
    }
}
# SharpChat
[![.NET Build](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml/badge.svg?branch=main)](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml)

SharpChat is a C# library that tries to offer functionality similar to [TypeChat](https://github.com/microsoft/TypeChat), making it seamless to construct natural language interfaces using types.
Schema enginerring is the new prompt engineering!

With SharpChat you can let ChatGPT call your full typed services!
Here's how it works in action:
```
[user] Hi, please order me matcha for 10€ and black tea for 20€. Immediately place the order.    
[call] GetPrice(Tea: Matcha)
[call] GetPrice(Tea: Black)
[call] PlaceOrder(Tea: Matcha   Weight: 122 g    Price: 10 EUR)
[call] PlaceOrder(Tea: Black    Weight: 465 g    Price: 20 EUR)
[chat] Your order for 122 grams of Matcha tea and 465 grams of Black tea has been placed successfully. Enjoy your tea!
```

## Installation

```
dotnet add package SharpChat
```

## Get Started

1. Annotate your logic with `[Description]` attributes to give the chatbot some context
```csharp
        [Description("Commit the purchase of some tea. Returns true if the order was successfully placed.")]
        public bool PlaceOrder(Order order)
        {
        ...
        }
```

`[Description]` attrbutes should also be used to annotate parameter types!
```csharp
[Description("A customer order of tea")]
public record Order([Description("The type of tea")] TeaType TeaType,
    [Description("The weight (quantity) of the tea type in grams")] float Grams,
    [Description("The order amount (price) in EUR")] float Price)
{
}
```

3. SharpChat is setup during initialization of your services.
Simply call `AddSharpChat` to register the functions you want the chatbot to use.

```csharp
var services = new ServiceCollection()
    .AddSharpChat((f, s) => f
        .RegisterFunction(s.GetRequiredService<IMyService>().myFunc1)
        .RegisterFunction(s.GetRequiredService<IMyService>().myFunc1))
    .BuildServiceProvider();
```

You can also expose all functions of your service at once:

```csharp
var services = new ServiceCollection()
    .AddSharpChat((f, s) => f
        .RegisterAllFunctions(s.GetRequiredService<IMyService>())
    .BuildServiceProvider();
```

3. Start a conversation, passing an OpenAI API client initialized with your API key.
For best results you should use the `gpt-4` model. To be more cost efficient `gpt-3.5-turbo-0613` can also be used.

```csharp
var factory = services.GetRequiredService<IConversationFactory>();
var client = new OpenAIClient(API_KEY);
var conversation = factory.StartConversation(client, "gpt-4");
```

SharpChat automatically exposes your registerd functions to the conversation, allowing the chatbot to call them!

4. Start chatting!

```csharp
var output = await conversation.Prompt("What is the meaning of life?");
```

**Take a look at `SharpChat.Examples.Shopping` and `SharpChat.Examples.Math` for inspiration on how to use SharpChat.**

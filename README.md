# SharpChat
[![.NET Build](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml/badge.svg?branch=main)](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml)

SharpChat is a C# library that tries to offer functionality similar to [TypeChat](https://github.com/microsoft/TypeChat), making it seamless to construct natural language interfaces using types.
Schema enginerring is the new prompt engineering!

## Installation

```
dotnet add package SharpChat
```

## Get Started

1. SharpChat is setup during initialization of your services.
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

2. Start a conversation, passing an OpenAI API client initialized with your API key.
For best results you should use the `gpt-4` model. To be more cost efficient `gpt-3.5-turbo-0613` can also be used.

```csharp
var factory = services.GetRequiredService<IConversationFactory>();
var client = new OpenAIClient(API_KEY);
var conversation = factory.StartConversation(client, "gpt-4");
```

SharpChat automatically exposes your registerd functions to the conversation, allowing the chatbot to call them!

3. Start chatting!

```csharp
var output = await conversation.Prompt("What is the meaning of life?");
```

**Take a look at `SharpChat.Examples.Shopping` and `SharpChat.Examples.Math` for inspiration on how to use SharpChat.**

# SharpChat
[![.NET Build](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml/badge.svg?branch=main)](https://github.com/everharder/SharpChat/actions/workflows/dotnet-build.yml)

SharpChat is a C# library that tries to offer functionality similar to [TypeChat](https://github.com/microsoft/TypeChat), making it seamless to construct natural language interfaces using types.
Schema enginerring is the new prompt engineering!

## Installation

```
dotnet add package SharpChat
```

## Get Started

SharpChat is setup during initialization of your services.
Simply call `AddSharpChat` to register the functions you want the chatbot to use.

```csharp

var services = new ServiceCollection()
    .AddSharpChat((f, _) => f
        .RegisterFunction(myFunc1)
        .RegisterFunction(myFunc2)
        ...
    .BuildServiceProvider();
```

Start a conversation, passing an OpenAI API client initialized with your API key.

```csharp
var factory = services.GetRequiredService<IConversationFactory>();
var client = new OpenAIClient(apikey);
var conversation = factory.StartConversation(client, model);
```

SharpChat automatically exposes your registerd functions to the conversation, allowing the chatbot to call them!
Now you can start chatting!

```csharp
var output = await conversation.Prompt("...");
```

**Take a look at `SharpChat.Examples.Shopping` and `SharpChat.Examples.Math` for inspiration on how to use SharpChat.**

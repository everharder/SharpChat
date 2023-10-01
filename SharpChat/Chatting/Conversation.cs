using Azure.AI.OpenAI;
using SharpChat.Functions;

namespace SharpChat.Chatting;

/// <summary>
/// A conversation flow with the chatbot.
/// Handles prompting history and function calling
/// </summary>
internal class Conversation : IConversation
{
    private readonly OpenAIClient client;
    private readonly ChatConfiguration configuration;
    private readonly IFunctionInvoker functionInvoker;
    private readonly List<ChatMessage> messages = new();
    private readonly IReadOnlyList<FunctionDefinition> functions;

    /// <summary>
    /// this field ensures that a prompt chain does not exceed the maximum call depth
    /// a chatbot deciding to recursively calling functions can quickly eat up your budget...
    /// </summary>
    private static AsyncLocal<int> functionCallCount = new AsyncLocal<int>();

    public Conversation(
        OpenAIClient client,
        ChatConfiguration configuration,
        IFunctionRegistry functionRegistry,
        IFunctionInvoker functionInvoker)
    {
        this.client = client;
        this.configuration = configuration;
        this.functionInvoker = functionInvoker;
        if (string.IsNullOrWhiteSpace(configuration.SystemPrompt))
        {
            throw new ArgumentException(nameof(configuration.SystemPrompt));
        }

        // add system prompt as first input
        ChatMessage systemMessage = new(role: ChatRole.System, content: configuration.SystemPrompt);
        messages.Add(systemMessage);

        functions = functionRegistry.GetRegisteredFunctions().AsReadOnly();
    }

    public async Task<string> Prompt(string input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException(nameof(input));
        }

        functionCallCount.Value = 0;

        // add prompt to messages
        messages.Add(new ChatMessage(role: ChatRole.User, content: input));

        // send current message and history to chatbot
        Azure.Response<ChatCompletions> response = await client.GetChatCompletionsAsync(configuration.Model, CreateConfig(), cancellationToken);

        // add response to history
        ChatMessage responseMessage = response.Value.Choices.First().Message;
        messages.Add(responseMessage);

        return await HandleResponse(responseMessage, cancellationToken);
    }

    private async Task<string> HandleResponse(ChatMessage response, CancellationToken cancellationToken)
    {
        // check if the bot wants to call a function
        if (response.FunctionCall != null)
        {
            if (functionCallCount.Value > configuration.MaxConsequtiveFunctionCalls)
            {
                throw new Exception($"Too many consequtive function calls (maximum: {configuration.MaxConsequtiveFunctionCalls}");
            }
            functionCallCount.Value += 1;
            object? result = functionInvoker.CallFunction(response.FunctionCall);

            // add the function call result to the history
            messages.Add(new ChatMessage(ChatRole.Function, result?.ToString())
            {
                Name = response.FunctionCall.Name,
            });

            // share the execution result with the chatbot
            Azure.Response<ChatCompletions> functionResultResponse = await client.GetChatCompletionsAsync(configuration.Model, CreateConfig(), cancellationToken);

            ChatMessage functionResultResponseMessage = functionResultResponse.Value.Choices.First().Message;
            messages.Add(functionResultResponseMessage);

            // recursive call, as response might be another function call
            return await HandleResponse(functionResultResponseMessage, cancellationToken);
        }

        return response.Content;
    }

    private ChatCompletionsOptions CreateConfig()
        => new ChatCompletionsOptions(messages)
        {
            Temperature = configuration.Temperature,
            Functions = functions.ToList(),
            FunctionCall = functionCallCount.Value == configuration.MaxConsequtiveFunctionCalls
                        ? FunctionDefinition.None
                        : FunctionDefinition.Auto
        };
}

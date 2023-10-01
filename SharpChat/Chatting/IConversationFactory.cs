using Azure.AI.OpenAI;

namespace SharpChat.Chatting;

/// <summary>
/// Factory for creating <see cref="IConversation"/>s
/// </summary>
public interface IConversationFactory
{
    /// <summary>
    /// Starts a new conversation with a language model
    /// </summary>
    IConversation StartConversation(OpenAIClient client, ChatConfiguration configuration);

    /// <summary>
    /// Starts a new conversation with a language model
    /// </summary>
    IConversation StartConversation(OpenAIClient client, string model);
}
namespace SharpChat.Chatting;

public interface IConversation
{
    /// <summary>
    /// Prompt the chatbot
    /// </summary>
    /// <returns>The chatbots response</returns>
    Task<string> Prompt(string input, CancellationToken cancellationToken = default);
}
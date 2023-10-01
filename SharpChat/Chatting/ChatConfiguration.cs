namespace SharpChat.Chatting;

/// <summary>
/// Configuration of the chat interactions
/// </summary>
/// <param name="Model">The model to use</param>
/// <param name="SystemPrompt">The system prompt to initialize the models behavior (purpose)</param>
/// <param name="Temperature">The creativity of the chatbot (0-2)</param>
/// <param name="MaxConsequtiveFunctionCalls">How many function calls should happen in a row</param>
public record ChatConfiguration(string Model, string? SystemPrompt = null, float Temperature = 0.2f, int MaxConsequtiveFunctionCalls = 3)
{
}

namespace SharpChat.Chatting
{
    /// <summary>
    /// Configuration of the chat interactions
    /// </summary>
    public class ChatConfiguration
    {
        /// <summary>
        /// The model to use (e.g. GPT-4, GPT-3.5-turbo)
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// The system prompt for the model (i.e. its purpose)
        /// </summary>
        public string SystemPrompt { get; }

        /// <summary>
        /// The creativeness of the chatbot (0-2)
        /// </summary>
        public float Temperature { get; }

        /// <summary>
        /// The number of allowed function calls in a row
        /// Each function call is a separate query to the OpenAI API (be catious with pricing!)
        /// </summary>
        public int MaxConsequtiveFunctionCalls { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ChatConfiguration"/>
        /// </summary>
        public ChatConfiguration(string Model, string SystemPrompt = null, float Temperature = 0.2f, int MaxConsequtiveFunctionCalls = 3)
        {
            this.Model = Model;
            this.SystemPrompt = SystemPrompt;
            this.Temperature = Temperature;
            this.MaxConsequtiveFunctionCalls = MaxConsequtiveFunctionCalls;
        }

    }
}
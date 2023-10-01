using Azure.AI.OpenAI;
using SharpChat.Functions;

namespace SharpChat.Chatting
{
    internal class ConversationFactory : IConversationFactory
    {
        private readonly IFunctionRegistry functionRegistry;
        private readonly IFunctionInvoker functionInvoker;

        public ConversationFactory(IFunctionRegistry functionRegistry, IFunctionInvoker functionInvoker)
        {
            this.functionRegistry = functionRegistry;
            this.functionInvoker = functionInvoker;
        }

        /// <inheritdoc/>
        public IConversation StartConversation(OpenAIClient client, string model)
            => StartConversation(client, new ChatConfiguration(model));

        /// <inheritdoc/>
        public IConversation StartConversation(OpenAIClient client, ChatConfiguration configuration)
            => new Conversation(client, configuration, functionRegistry, functionInvoker);
    }
}

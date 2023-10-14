using System.Threading;
using System.Threading.Tasks;

namespace SharpChat.Chatting
{
    /// <summary>
    /// Interface for a chatbot conversation
    /// </summary>
    public interface IConversation
    {
        /// <summary>
        /// Prompt the chatbot
        /// </summary>
        /// <returns>The chatbots response</returns>
        Task<string> Prompt(string input, CancellationToken cancellationToken = default);
    }
}
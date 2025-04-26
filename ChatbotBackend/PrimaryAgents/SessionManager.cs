using ChatbotBackend.Models;
using ChatbotBackend.Plugins;
using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace ChatbotBackend.PrimaryAgents
{
    public class SessionManager : ISessionManager
    {
        private readonly Kernel kernel;
        private readonly IChatCompletionService chatCompletion;
        private readonly ChatHistory history;

        public SessionManager(Kernel kernel, SessionManagerPlugin plugin)
        {
            this.kernel = kernel.Clone();
            chatCompletion = this.kernel.GetRequiredService<IChatCompletionService>();
            this.kernel.ImportPluginFromObject(plugin);
            history = new ChatHistory(Prompts.AUTH_PROMPT);
        }

        public async Task<CommunicationFormat> TryGetUsersession(string prompt)
        {
            history.AddUserMessage(prompt);
            var response = await chatCompletion.GetChatMessageContentAsync(history,
                new AzureOpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() },
                kernel: kernel);
            history.Add(response);

            return new CommunicationFormat
            {
                Message = response.Content,
                User = nameof(SessionManager)
            };
        }
    }
}

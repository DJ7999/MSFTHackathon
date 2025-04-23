using ChatbotBackend.Models;
using ChatbotBackend.Plugins;
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
            history = new ChatHistory("You are responsible making existing user signin and making new user sign up and ask them necessary information to complete this." +
                "necessary information is username and password" +
                "if plugin respond with non zero value it means signin or signup is successfull else responde with if Signin failed Username or password is incorrect if" +
                "SignUp failed Username is already in use  note -- you will only act on the given information");
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


using ChatbotBackend.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace ChatbotBackend.Agent
{
    public class FinanceAgent : IAgent
    {
        private readonly IChatCompletionService chatCompletionService;
        private ChatHistory chatHistory;
        private readonly Kernel agentKernel;
        private readonly AzureOpenAIPromptExecutionSettings Executionsetting;
        public FinanceAgent(Kernel kernel)
        {
            this.agentKernel = kernel.Clone();
            agentKernel.ImportPluginFromType<FinancePlugin>();
            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            chatHistory = new ChatHistory($@"You are an intelligent Finance advisor Agent specialized in 
                                          finance-related queries.
                                        Important: Ensure that the selection is based solely on the information provided 
                                        in the user's query. Do not make assumptions beyond the given data.
                                        Reply in markdown format whenever possible");
            Executionsetting = new AzureOpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
        }
        public async Task<CommunicationFormat> GetAgentResponseAsync(string message)
        {
            chatHistory.AddUserMessage(message);
            var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, Executionsetting, agentKernel);
            chatHistory.AddAssistantMessage(response.Content);
            return new CommunicationFormat()
            {
                Message = response.ToString(),
                User = nameof(GoalPlanningAgent),
            };
        }
    }
}

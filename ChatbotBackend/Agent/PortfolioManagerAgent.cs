using ChatbotBackend.Models;
using ChatbotBackend.Plugins;
using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace ChatbotBackend.Agent
{
    public class PortfolioManagerAgent : IAgent
    {
        private readonly IChatCompletionService chatCompletionService;
        private ChatHistory chatHistory;
        private readonly AzureOpenAIPromptExecutionSettings Executionsetting;
        private readonly Kernel kernel;
        public PortfolioManagerAgent(Kernel kernel, PortfolioPlugin portfolioPlugin, UserInfoPlugin userInfoPlugin)
        {
            this.kernel = kernel.Clone();
            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            this.kernel.ImportPluginFromObject(portfolioPlugin);
            this.kernel.ImportPluginFromObject(userInfoPlugin);
            this.kernel.ImportPluginFromType<FinancePlugin>();
            chatHistory = new ChatHistory(Prompts.AGENT_PORTFOLIO_PROMPT);
            FunctionChoiceBehaviorOptions functionChoiceBehaviorOptions = new FunctionChoiceBehaviorOptions()
            {
                AllowConcurrentInvocation = false,
                AllowParallelCalls = false
            };
            Executionsetting = new AzureOpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: functionChoiceBehaviorOptions),


            };
        }

        public async Task<CommunicationFormat> GetAgentResponseAsync(string message)
        {
            chatHistory.AddUserMessage(message);
            var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, Executionsetting, kernel);
            chatHistory.AddAssistantMessage(response?.Content ?? "");
            return new CommunicationFormat()
            {
                User = nameof(RetirementPlanningAgent),
                Message = response.ToString()
            };
        }
    }
}


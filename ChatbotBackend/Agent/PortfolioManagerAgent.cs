using ChatbotBackend.Plugins;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel;
using ChatbotBackend.Models;

namespace ChatbotBackend.Agent
{
    public class PortfolioManagerAgent :IAgent
    {
        private readonly IChatCompletionService chatCompletionService;
        private ChatHistory chatHistory;
        private readonly AzureOpenAIPromptExecutionSettings Executionsetting;
        private readonly Kernel kernel;
        public PortfolioManagerAgent(Kernel kernel, PortfolioPlugin portfolioPlugin, UserInfoPlugin userInfoPlugin, MarketReaserchPlugin marketReaserchPlugin)
        {
            this.kernel = kernel.Clone();
            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            this.kernel.ImportPluginFromObject(portfolioPlugin);
            this.kernel.ImportPluginFromObject(userInfoPlugin);
            this.kernel.ImportPluginFromObject(marketReaserchPlugin);
            this.kernel.ImportPluginFromType<FinancePlugin>();
            chatHistory = new ChatHistory($"You are a portfolio manager agent help user with their portfolio management you can also  perform a fundamental analysis of company by executing the following steps:​\" +\r\n                \"Identify the sector to which the company belongs.​\" +\r\n                \"Retrieve various metrics using the Financial Growth plugin, Profibility plugin, Valuation metric plugin, Leverage And Solvency Metric Plugin,\" +\r\n                \"Liquidity And Effiiciency Metric Plugin, CashFlow Health Metric Plugin, Ownership And Red Flags Metric Plugin.​\" +\r\n                \"after getting the all the metrices use verdict pluginfor  the verdict along with a detailed explanation.\" +\r\n                $\"your response should include verdict from {{typeof(StockVerdict)}} at top and then explaination\"" +
                $"Note while calling plugin you will use ticker for comapny supported by yahoo finance");
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


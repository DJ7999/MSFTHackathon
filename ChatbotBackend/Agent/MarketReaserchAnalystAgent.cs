using ChatbotBackend.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace ChatbotBackend.Agent
{
    public class MarketReaserchAnalystAgent : IAgent
    {
        private readonly Kernel kernel;
        private readonly IChatCompletionService completionService;
        private readonly PromptExecutionSettings executionSettings;
        ChatHistory history;
        public MarketReaserchAnalystAgent(Kernel kernel, MarketReaserchPlugin marketReaserchPlugin)
        {
            this.kernel = kernel.Clone();
            this.kernel.ImportPluginFromObject(marketReaserchPlugin);
            completionService = kernel.GetRequiredService<IChatCompletionService>();
            executionSettings = new AzureOpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions()
                {
                    AllowConcurrentInvocation = false,
                    AllowParallelCalls = false
                }),
            };
            history = new ChatHistory("Your task to perform Please perform a fundamental analysis of company by executing the following steps:​" +
                "Identify the sector to which the company belongs.​" +
                "Retrieve various metrics using the Financial Growth plugin, Profibility plugin, Valuation metric plugin, Leverage And Solvency Metric Plugin," +
                "Liquidity And Effiiciency Metric Plugin, CashFlow Health Metric Plugin, Ownership And Red Flags Metric Plugin.​" +
                "after getting the all the metrices use verdict pluginfor  the verdict along with a detailed explanation." +
                $"your response should include verdict from {typeof(StockVerdict)} at top and then explaination");
        }
        public async Task<CommunicationFormat> GetAgentResponseAsync(string message)
        {
            history.AddUserMessage(message);
            var response = await completionService.GetChatMessageContentAsync(history, executionSettings, kernel: kernel);
            return new CommunicationFormat
            {
                Message = response.ToString(),
                User = nameof(MarketReaserchAnalystAgent),
            };
        }
    }
}

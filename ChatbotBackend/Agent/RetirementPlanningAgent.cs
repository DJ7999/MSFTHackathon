
using ChatbotBackend.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace ChatbotBackend.Agent
{
    public class RetirementPlanningAgent : IAgent
    {
        private readonly IChatCompletionService chatCompletionService;
        private ChatHistory chatHistory;
        private readonly AzureOpenAIPromptExecutionSettings Executionsetting;
        private readonly Kernel kernel;
        public RetirementPlanningAgent(Kernel kernel, RetirementPlugin retirementPlugin, UserInfoPlugin userInfoPlugin)
        {
            this.kernel = kernel.Clone();
            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            this.kernel.ImportPluginFromObject(retirementPlugin);
            this.kernel.ImportPluginFromObject(userInfoPlugin);
            this.kernel.ImportPluginFromType<FinancePlugin>();
            chatHistory = new ChatHistory($"You are a highly intelligent Retirement Planning Agent specializing in helping users with retirement planning queries."+
"user current retirement plan can be retrieved using retirementPlugin if there iis any and users info can be retrieved using userinfo plugin if available"+
"When a user asks about retirement planning, please gather the following information before creating a plan:" +
"- monthly expenditure"+
"- Desired retirement age or time frame" +
"Your process should be:" +
"calculate yearly expense" +
"Calculate the FIRE number considering yearly expense" +
"calculate inflation adjusted fire number" +
"Create an SIP recommendation: Based on the inflation adjusted FIRE number" +
"confirm with user before saving it also if user provide his monthly expenditure and salary save  it.  Note : behave more human like and give short responses");
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

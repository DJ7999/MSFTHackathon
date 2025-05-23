﻿
using ChatbotBackend.Models;
using ChatbotBackend.Plugins;
using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

namespace ChatbotBackend.Agent
{

    public class GoalPlanningAgent : IAgent
    {
        private readonly IChatCompletionService chatCompletionService;
        private ChatHistory chatHistory;
        private readonly Kernel kernel;
        private readonly AzureOpenAIPromptExecutionSettings Executionsetting;
        public GoalPlanningAgent(Kernel kernel, GoalPlugin goalPlugin)
        {
            this.kernel = kernel.Clone();
            this.kernel.ImportPluginFromType<FinancePlugin>();
            this.kernel.ImportPluginFromObject(goalPlugin);
            Executionsetting = new AzureOpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions()
                {
                    AllowConcurrentInvocation = false,
                    AllowParallelCalls = false
                })
            };
            chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            chatHistory = new ChatHistory(Prompts.GOAL_PLANNING_PROMPT);
        }
        public async Task<CommunicationFormat> GetAgentResponseAsync(string message)
        {
            chatHistory.AddUserMessage(message);
            var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory, Executionsetting, kernel);
            chatHistory.AddAssistantMessage(response.Content);
            return new CommunicationFormat()
            {
                Message = response.ToString(),
                User = nameof(GoalPlanningAgent),
            };
        }
    }
}

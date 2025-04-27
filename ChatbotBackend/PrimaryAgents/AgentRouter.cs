using ChatbotBackend.Agent;
using ChatbotBackend.Plugins;
using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Newtonsoft.Json;

namespace ChatbotBackend.PrimaryAgents
{
    public class AgentRouter : IAgentRouter
    {
        private readonly Func<string, IAgent> _agentFactory;
        private readonly AzureOpenAIPromptExecutionSettings routerPromptExecutionsetting;
        readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;
        ChatHistory _history;
        public AgentRouter(Kernel kernel, Func<string, IAgent> agentFactory)
        {
            _kernel = kernel.Clone();
            _kernel.ImportPluginFromType<FinancePlugin>();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _history = new ChatHistory(Prompts.AGENT_ROUTER_PROMPT);
            _agentFactory = agentFactory;
            routerPromptExecutionsetting = new AzureOpenAIPromptExecutionSettings
            {
                ResponseFormat = typeof(AgentResponse)
            };
        }

        public static IEnumerable<string> GetAllAgents()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAgent).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .Select(type => type.Name);
        }

        public async Task<RouterResponse> GetAgentAsync(string message)
        {
            _history.AddUserMessage(message);

            var responseContent = await _chatCompletionService.GetChatMessageContentAsync(_history, executionSettings: routerPromptExecutionsetting, _kernel);



            try
            {
                var response = JsonConvert.DeserializeObject<AgentResponse>(responseContent.ToString());
                List<AgentPrompt> agents = new List<AgentPrompt>();
                if (response?.Agents != null)
                {
                    foreach (var Agent in response.Agents)
                    {
                        agents.Add(new AgentPrompt
                        {
                            Agent = _agentFactory(Agent.AgentName),
                            Prompt = Agent.AgentPrompt,
                        });
                    }

                }
                return new RouterResponse
                {
                    Agents = agents,
                    UserMessage = response?.UserMessage ?? ""
                };



            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateRouterMemory(string message, string author)
        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _history.Add(new ChatMessageContent(AuthorRole.Assistant, message) { AuthorName = author });
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        // Define a class that matches the expected structure of the JSON response
        public class AgentResponse
        {
            public List<AgentInfo>? Agents { get; set; }
            public string? UserMessage { get; set; }
        }
        public class AgentInfo
        {
            public string? AgentName { get; set; }

            public string? AgentPrompt { get; set; }
        }
        public class RouterResponse
        {
            public List<AgentPrompt>? Agents { get; set; }
            public string? UserMessage { get; set; }
        }
        public class AgentPrompt
        {
            public IAgent Agent { get; set; }

            public string? Prompt { get; set; }
        }
    }
}

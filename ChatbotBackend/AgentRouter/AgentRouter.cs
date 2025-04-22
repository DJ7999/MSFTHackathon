using ChatbotBackend.Agent;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Newtonsoft.Json;

namespace ChatbotBackend.AgentRouter
{
    public class AgentRouter : IAgentRouter
    {
        private readonly Func<string, IAgent> _agentFactory;
        private readonly AzureOpenAIPromptExecutionSettings routerPromptExecutionsetting;
        readonly IChatCompletionService _chatCompletionService;
        ChatHistory _history;
        public AgentRouter(Kernel kernel, Func<string, IAgent> agentFactory)
        {
            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _history = new ChatHistory($@"You are an intelligent Agent Router specialized in directing 
                                          finance-related queries to the appropriate agent.
                                          Based on the user's input, select the most suitable agent from the following list:
                                          {string.Join("| ", GetAllAgents())}
                                          Respond with agent name if you think user query matches to some agent other wise respond message for user 
                                          mentioning supported agents
                                        Important: Ensure that the selection is based solely on the information provided 
                                        in the user's query. Do not make assumptions beyond the given data.");
            _agentFactory = agentFactory;
            routerPromptExecutionsetting = new AzureOpenAIPromptExecutionSettings
            {
                ResponseFormat = typeof(AgentResponse)
            };
        }

        private IEnumerable<string> GetAllAgents()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAgent).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .Select(type => type.Name);
        }

        public async Task<(IAgent? Agent, string ResponseMessage)> GetAgentAsync(string message)
        {
            _history.AddUserMessage(message);

            var responseContent = await _chatCompletionService.GetChatMessageContentAsync(_history, executionSettings: routerPromptExecutionsetting);

            IAgent agent = null;
            string responseMessage = string.Empty;

            try
            {
                var response = JsonConvert.DeserializeObject<AgentResponse>(responseContent.ToString());

                if (!string.IsNullOrEmpty(response.Agent))
                {
                    agent = _agentFactory(response.Agent);

                    if (agent == null)
                    {
                        responseMessage = "Unable to complete your request at the moment.";
                    }
                }
                else
                {
                    responseMessage = response.Message ?? "An unexpected error occurred.";
                }
            }
            catch (JsonException ex)
            {
                responseMessage = $"Failed to parse the response: {ex.Message}";
            }
            catch (Exception ex)
            {
                responseMessage = $"An error occurred: {ex.Message}";
            }

            return (agent, responseMessage);
        }

        public void UpdateRouterMemory(string message, string author)
        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _history.Add(new ChatMessageContent(AuthorRole.Assistant, message) { AuthorName = author });
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        // Define a class that matches the expected structure of the JSON response
        private class AgentResponse
        {
            public string? Agent { get; set; }
            public string? Message { get; set; }
        }
    }
}

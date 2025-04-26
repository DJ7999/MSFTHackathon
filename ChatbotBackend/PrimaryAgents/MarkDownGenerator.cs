using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ChatbotBackend.PrimaryAgents
{
    public class MarkDownGenerator : IMarkDownGenerator
    {
        private readonly IChatCompletionService agent;

        public MarkDownGenerator(Kernel kernel)
        {
            
            agent = kernel.GetRequiredService<IChatCompletionService>();
        }
        public async Task<string> GenerateMarkdown(string markdown)
        {
            ChatHistory history = new ChatHistory(Prompts.MarkdownPrompt);

            history.AddAssistantMessage(markdown);
            var response = await agent.GetChatMessageContentAsync(history);
            return response.ToString();
        }
    }
}

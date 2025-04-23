using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ChatbotBackend.PrimaryAgents
{
    public class MarkDownGenerator : IMarkDownGenerator
    {
        private readonly Kernel markDownKernel;
        private readonly IChatCompletionService agent;

        public MarkDownGenerator(Kernel kernel)
        {
            markDownKernel = kernel;
            agent = kernel.GetRequiredService<IChatCompletionService>();
        }
        public async Task<string> GenerateMarkdown(string markdown)
        {
            ChatHistory history = new ChatHistory(@"You are an assistant designed to present information in a clear and organized manner. When responding to user messages, structure your replies to enhance readability and engagement. Utilize appropriate formatting techniques such as headings, bullet points, tables, and visual representations like graphs when they aid in conveying information effectively. Ensure that your responses are informative and well-structured, without referencing the formatting methods used.");


            history.AddAssistantMessage(markdown);
            var response = await agent.GetChatMessageContentAsync(history);
            return response.ToString();
        }
    }
}

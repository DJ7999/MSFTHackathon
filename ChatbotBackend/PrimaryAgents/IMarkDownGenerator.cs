namespace ChatbotBackend.PrimaryAgents
{
    public interface IMarkDownGenerator
    {
        Task<string> GenerateMarkdown(string markdown);
    }
}

namespace ChatbotBackend.MarkDown
{
    public interface IMarkDownGenerator
    {
        Task<string> GenerateMarkdown(string markdown);
    }
}

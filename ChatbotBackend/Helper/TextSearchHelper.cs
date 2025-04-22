using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;

namespace ChatbotBackend.Helper
{
    public class TextSearchHelper
    {
        private readonly Kernel kernel;
        private readonly ITextSearch textSearch;
        private readonly ILogger logger;
        public TextSearchHelper(Kernel kernel, ILogger<TextSearchHelper> logger)
        {
            this.kernel = kernel;
            textSearch = kernel.GetRequiredService<ITextSearch>();
            this.logger = logger;
        }
        public async Task<string> TextSearchAsync(string text)
        {
            logger.LogInformation(text);
            try
            {
                var response = await textSearch.SearchAsync(text, new() { Top = 10, Skip = 0 });
                return await response.Results.AggregateAsync((current, next) => $"{current}\n{next}");
            }
            catch (Exception ex)
            {
                return "";
            }

        }
    }
}

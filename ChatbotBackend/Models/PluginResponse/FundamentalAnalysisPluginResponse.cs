namespace ChatbotBackend.Models.PluginResponse
{
    public class FundamentalAnalysisPluginResponse
    {
        public string CompanyName { get; set; }
        public StockVerdict StockVerdict { get; set; }
        public string Message { get; set; }
    }
}

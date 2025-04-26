namespace ChatbotBackend.Models
{
    public class StockVerdict
    {
        public CompanyFinancials Financials { get; set; }
        public int Score { get; set; }
        public string Verdict { get; set; }
    }
}

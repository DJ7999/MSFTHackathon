using ChatbotBackend.Models;

namespace ChatbotBackend.Services
{
    public class StockServices
    {
        public static async Task<double?> GetLatestPriceAsync(string symbol)
        {
            try
            {
                CompanyFinancials companyFinancial = await ScreenerFinanceScraper.ScrapeCompanyAsync(symbol);
                
                return companyFinancial.CurrentPrice;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock price: {ex.Message}");
                return null;
            }
        }
        public static StockVerdict Verdict(CompanyFinancials data)
        {
            int score = 0;

            // 📊 Valuation (35)
            score += data.PE < 15 ? 20 : data.PE < 25 ? 15 : data.PE < 30 ? 10 : 5;

            double priceToBook = data.BookValue > 0 ? data.CurrentPrice / data.BookValue : 0;
            score += priceToBook < 2 ? 15 : priceToBook < 3 ? 10 : 5;

            // 💰 Dividend Yield (15)
            score += data.DividendYield > 2 ? 15
                  : data.DividendYield > 1 ? 10
                  : data.DividendYield > 0.5 ? 7 : 3;

            // 📈 Profitability (30)
            score += data.ROCE > 20 ? 15 : data.ROCE > 10 ? 10 : data.ROCE > 5 ? 5 : 2;
            score += data.ROE > 20 ? 15 : data.ROE > 10 ? 10 : data.ROE > 5 ? 5 : 2;

            // ⚠️ Risk/Volatility (20)
            double highLowRange = data.High52 - data.Low52;
            double midpoint = data.Low52 + highLowRange / 2;
            double diff = Math.Abs(data.CurrentPrice - midpoint);
            double riskScore = highLowRange > 0 ? (1.0 - (diff / highLowRange)) : 0;
            score += (int)(riskScore * 20); // normalize to 0–20

            // 🏁 Final Verdict
            string verdict = score >= 80 ? "Buy"
                          : score >= 60 ? "Hold/Watch"
                          : "Exit";

            return new StockVerdict
            {
                Financials = data,
                Score = score,
                Verdict = verdict
            };


        }
        public async static Task<StockVerdict> Analyze(string ticker)
        {
            try
            {
                var companyFinancial = await ScreenerFinanceScraper.ScrapeCompanyAsync(ticker);
                var verdict = Verdict(companyFinancial);
                return verdict;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception ("we are currently not supporting this investment");
            }
            
        }
    }
}

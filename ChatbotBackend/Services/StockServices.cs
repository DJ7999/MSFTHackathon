using YahooFinanceApi;

namespace ChatbotBackend.Services
{
    public class StockServices
    {
        public static async Task<float?> GetLatestPriceAsync(string symbol)
        {
            try
            {
                var securities = await Yahoo.Symbols(symbol)
                                             .Fields(Field.Symbol, Field.RegularMarketPrice)
                                             .QueryAsync();

                var security = securities[symbol];
                var price =  security[Field.RegularMarketPrice];
                return (float)price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock price: {ex.Message}");
                return null;
            }
        }
    }
}

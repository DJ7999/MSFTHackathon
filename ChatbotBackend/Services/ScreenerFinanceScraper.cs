using ChatbotBackend.Models;
using HtmlAgilityPack;
using System.Globalization;

public class ScreenerFinanceScraper
{
    public static async Task<CompanyFinancials> ScrapeCompanyAsync(string ticker)
    {
        using HttpClient client = new HttpClient();

        var url = $"https://www.screener.in/company/{ticker}/consolidated/";
        var html = await client.GetStringAsync(url);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var textContent = htmlDoc.DocumentNode.InnerText;

        double ParseValue(string label, bool isPercent = false)
        {
            try
            {
                int idx = textContent.IndexOf(label, StringComparison.OrdinalIgnoreCase);
                if (idx == -1) return 0;

                var snippet = textContent.Substring(idx + label.Length, 50);
                var match = System.Text.RegularExpressions.Regex.Match(snippet, @"[-+]?[0-9]*\,?[0-9]+\.?[0-9]*");

                if (match.Success)
                {
                    string clean = match.Value.Replace(",", "");
                    double value = double.Parse(clean, CultureInfo.InvariantCulture);
                    return isPercent ? value : value;
                }
            }
            catch { }

            return 0;
        }

        // Special handling for 52-week High / Low
        (double high, double low) ParseHighLow()
        {
            try
            {
                var idx = textContent.IndexOf("High / Low", StringComparison.OrdinalIgnoreCase);
                if (idx == -1) return (0, 0);

                var snippet = textContent.Substring(idx, 100);
                var matches = System.Text.RegularExpressions.Regex.Matches(snippet, @"[-+]?[0-9]*\,?[0-9]+\.?[0-9]*");

                if (matches.Count >= 2)
                {
                    double high = double.Parse(matches[0].Value.Replace(",", ""), CultureInfo.InvariantCulture);
                    double low = double.Parse(matches[1].Value.Replace(",", ""), CultureInfo.InvariantCulture);
                    return (high, low);
                }
            }
            catch { }

            return (0, 0);
        }

        var (high52, low52) = ParseHighLow();

        var result = new CompanyFinancials
        {
            CurrentPrice = ParseValue("Current Price"),
            High52 = high52,
            Low52 = low52,
            PE = ParseValue("Stock P/E"),
            BookValue = ParseValue("Book Value"),
            DividendYield = ParseValue("Dividend Yield", isPercent: true),
            ROCE = ParseValue("ROCE", isPercent: true),
            ROE = ParseValue("ROE", isPercent: true)
        };

        return result;
    }

}

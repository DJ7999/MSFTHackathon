using ChatbotBackend.Models;
using ChatbotBackend.Models.DTO;
using ChatbotBackend.Repository;
using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class PortfolioPlugin
    {
        private readonly IPortfolioRepository repository;
        public PortfolioPlugin(IPortfolioRepository portfolioRepository)
        {
            repository = portfolioRepository;
        }

        [KernelFunction]
        [Description("add investment to portfolio ")]
        public async Task AddInvestmentToPortfolio(
        [Description("ticker value of asset name supported by yfinance")] string ticker,
        [Description("Quantity of assets bought")] int quantity)
        {
            await repository.AddUpdatePortfolio(new PortfolioDto { AssetName = ticker, Quantity = quantity });
        }

        [KernelFunction]
        [Description("remove oor sell investment to portfolio ")]
        public async Task RemoveInvestmentToPortfolio(
        [Description("ticker value of asset name supported by yfinance")] string ticker,
        [Description("Quantity of assets sold")] int quantity)
        {
            await repository.RemoveUpdatePortfolio(new PortfolioDto { AssetName = ticker, Quantity = quantity });
        }

        [KernelFunction]
        [Description("get all investment in user portfolio")]
        public async Task<List<PortfolioDto>> GetPortfolio()
        {
            var result = await repository.GetPortfolioDto();
            return result;
        }

        [KernelFunction]
        [Description("You can perform fundamental analysis on the given stock name and pass a verdict ")]
        public async Task<StockVerdict> CompanyVerdictPlugin([Description("Ticker is a company ticker supported by Screener website")] string ticker)
        {
            var result = await StockServices.Analyze(ticker);
            return result;
        }


    }
}

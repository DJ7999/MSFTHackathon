using ChatbotBackend.Models.DTO;
using ChatbotBackend.Repository;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class PortfolioPlugin
    {
        private readonly IPortfolioRepository repository;
        public PortfolioPlugin(IPortfolioRepository portfolioRepository) { 
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
    }
}

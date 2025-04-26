using ChatbotBackend.EntityFramework;
using ChatbotBackend.Models;
using ChatbotBackend.Models.DbModel;
using ChatbotBackend.Models.DTO;
using ChatbotBackend.Services;

namespace ChatbotBackend.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;
        private readonly UserContext _userContext;
        // Injecting AppDbContext via constructor for Dependency Injection
        public PortfolioRepository(AppDbContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // Asynchronous method to get goals for a specific user
        private async Task<List<Portfolio>> GetPortfolio()
        {
            int userId = ChatHub._connectionUserMap[_userContext.Id];
            return _context.Portfolios.Where(g => g.UserId == userId).ToList();
        }

        public async Task AddUpdatePortfolio(PortfolioDto portfolio)
        {
            var investments = await GetPortfolio();
            var investment = investments.Where(i => i.AssetName == portfolio.AssetName).SingleOrDefault();
            if (investment == null)
            {
                investment = new Portfolio
                {
                    AssetName = portfolio.AssetName,
                    Quantity = portfolio.Quantity,
                    UserId = ChatHub._connectionUserMap[_userContext.Id]
                };
                await _context.Portfolios.AddAsync(investment);
            }
            else
            {
                investment.Quantity += portfolio.Quantity;
                _context.Portfolios.Update(investment);
            }
            await _context.SaveChangesAsync(); // Save changes to the database
        }
        public async Task RemoveUpdatePortfolio(PortfolioDto portfolio)
        {
            var investments = await GetPortfolio();
            var investment = investments.Where(i => i.AssetName == portfolio.AssetName).SingleOrDefault();
            if (investment == null)
            {
                return;
            }
            if (investment.Quantity > portfolio.Quantity)
            {
                investment.Quantity += portfolio.Quantity;
                _context.Portfolios.Update(investment);
            }
            else
            {
                _context.Portfolios.Remove(investment);
            }
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        public async Task<List<PortfolioDto>> GetPortfolioDto()
        {
            var postfolioModels = await GetPortfolio();
            return postfolioModels.Select(p => new PortfolioDto { AssetName = p.AssetName, Quantity = p.Quantity, Value = GetEachStockValue(p.AssetName) * p.Quantity }).ToList();
        }

        private double GetEachStockValue(string ticker)
        {
            var price = StockServices.GetLatestPriceAsync(ticker).Result;
            if (!price.HasValue) throw new Exception("unable to find market value of asset ticker");
            return price.Value;
        }
    }
}

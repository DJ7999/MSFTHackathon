using ChatbotBackend.Models.DTO;

namespace ChatbotBackend.Repository
{
    public interface IPortfolioRepository
    {
        public Task AddUpdatePortfolio(PortfolioDto portfolio);
        public Task RemoveUpdatePortfolio(PortfolioDto portfolio);
        public Task<List<PortfolioDto>> GetPortfolioDto();
    }
}

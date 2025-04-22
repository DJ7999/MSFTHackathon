using ChatbotBackend.Models.DTO;

namespace ChatbotBackend.Repository
{
    public interface IRetirementRepository
    {
        public Task<RetirementDto> GetRetirementPlan();
        public Task AddUpdateRetirementPlan(RetirementDto retirementDto);
        public Task RemoveRetirementPlan();
    }
}

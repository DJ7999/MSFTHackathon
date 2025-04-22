using ChatbotBackend.Models.DTO;
using ChatbotBackend.Repository;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class RetirementPlugin
    {
        private readonly IRetirementRepository _retirementRepository;
        public RetirementPlugin(IRetirementRepository retirementRepository) { 
        _retirementRepository = retirementRepository;
        }

        [KernelFunction]
        [Description("Calculates the FIRE number based on annual expenses and withdrawal rate.")]
        public Task<double> CalculateFireNumberAsync(
        [Description("Expected annual expenses in after inflation adjustment.")] double annualExpenses,
        [Description("Safe withdrawal rate (default is 4%).")] double withdrawalRate = 4)
        {
            double fireNumber = annualExpenses / (withdrawalRate / 100);
            return Task.FromResult(Math.Round(fireNumber, 2));
        }

        [KernelFunction]
        [Description("Get you existing retirement plan if there is any")]
        public async Task<RetirementDto> GetRetirementPlan()
        {
            return await _retirementRepository.GetRetirementPlan();
        }

        [KernelFunction]
        [Description("Remove Existing retirement plan if there is any")]
        public async Task DeleteRetirementPlan()
        {
            await _retirementRepository.RemoveRetirementPlan();
        }

        [KernelFunction]
        [Description("Add or update retirement plan")]
        public async Task AddUpdateRetirementPlan(RetirementDto retirementDto)
        {
            await _retirementRepository.AddUpdateRetirementPlan(retirementDto);
        }
    }
}

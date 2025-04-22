using ChatbotBackend.Models.DTO;
using ChatbotBackend.Repository;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class GoalPlugin
    {
        private readonly IGoalRepository goalRepository;
        public GoalPlugin(IGoalRepository goalRepository) {
            this.goalRepository = goalRepository;
        }


        [KernelFunction]
        [Description("creates or updates existing goals name change is not allowed you have to create new goal and delete existing")]
        public async Task UpdateGoals( GoalDto goal)
        {
            await goalRepository.AddUpdateGoal(goal);
        }

        [KernelFunction]
        [Description("remove goal based on goal name")]
        public async Task DeleteGoal(string goal)
        {
            await goalRepository.RemoveGoal(goal);
        }

        [KernelFunction]
        [Description("get all goals")]
        public async Task<List<GoalDto>> GetGoals(string goal)
        {
            return await goalRepository.GetGoals();
        }
    }
}

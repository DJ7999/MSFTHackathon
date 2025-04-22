using ChatbotBackend.Models.DTO;

namespace ChatbotBackend.Repository
{
    public interface IGoalRepository
    {
        public Task<List<GoalDto>> GetGoals();
        public Task AddUpdateGoal(GoalDto goal);
        public Task RemoveGoal(string goal);
    }
}

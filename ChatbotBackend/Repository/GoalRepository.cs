using ChatbotBackend.EntityFramework;
using ChatbotBackend.Models;
using ChatbotBackend.Models.DbModel;
using ChatbotBackend.Models.DTO;

namespace ChatbotBackend.Repository
{
    public class GoalRepository : IGoalRepository
    {
        private readonly AppDbContext _context;
        private readonly UserContext _userContext;
        // Injecting AppDbContext via constructor for Dependency Injection
        public GoalRepository(AppDbContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // Asynchronous method to get goals for a specific user
        public async Task<List<GoalDto>> GetGoals()
        {
            var goals = await GetGoalModels();
            return goals.Select(g => new GoalDto { Name = g.Name, TargetAmount = g.TargetAmount, requiredSip = g.requiredSip }).ToList();
        }
        public async Task AddUpdateGoal(GoalDto goal)
        {
            try
            {
                var goals = await GetGoalModels();
                var existingGoal = goals.Where(g => g.Name == goal.Name).SingleOrDefault();
                if (existingGoal != null)
                {
                    existingGoal.TargetAmount = goal.TargetAmount;
                    existingGoal.requiredSip = goal.requiredSip;
                    _context.Update(existingGoal);
                }
                else
                {
                    await _context.AddAsync(new Goal
                    { Name = goal.Name, TargetAmount = goal.TargetAmount, requiredSip = goal.requiredSip, UserId = ChatHub._connectionUserMap[_userContext.Id] });
                }
                _context.SaveChanges();
            }
            catch (Exception ex) { }
        }

        public async Task RemoveGoal(string goal)
        {
            var goals = await GetGoalModels();
            var existingGoal = goals.Where(g => string.Equals(g.Name, goal, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            if (existingGoal != null)
            {

                _context.Remove(existingGoal);
                _context.SaveChanges();
            }
        }
        private async Task<List<Goal>> GetGoalModels()
        {
            int userId = ChatHub._connectionUserMap[_userContext.Id];
            return _context.Goals.Where(g => g.UserId == userId).ToList();
        }
    }
}

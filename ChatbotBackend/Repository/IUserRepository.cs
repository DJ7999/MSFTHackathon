using ChatbotBackend.Models.DTO;

namespace ChatbotBackend.Repository
{
    public interface IUserRepository
    {
        public Task<UserDto> GetUser();
        public Task<int> SignIn(string username, string password);
        public Task<int> SignUp(string username, string password);
        public Task UpdateUserSalary(float salary);
        public Task UpdateUserMonthlyExpense(float monthlyExpense);
    }
}

using ChatbotBackend.Models.DTO;
using ChatbotBackend.Repository;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class UserInfoPlugin
    {
        private readonly IUserRepository _userRepository;
        public UserInfoPlugin(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [KernelFunction]
        [Description("Get user info like monthly expense, salary and name")]
        public async Task<UserDto> GetUserInfo()
        {
            return await _userRepository.GetUser();
        }

        [KernelFunction]
        [Description("update user monthly expense")]
        public async Task UpdateUserExpense(
            [Description("users monthly expense.")] float monthlyExpense)
        {
            await _userRepository.UpdateUserMonthlyExpense(monthlyExpense);
        }

        [KernelFunction]
        [Description("update user salary monthly")]
        public async Task UpdateUserSalary(
            [Description("users monthly salary.")] float monthlysalary)
        {
            await _userRepository.UpdateUserSalary(monthlysalary);
        }
    }
}

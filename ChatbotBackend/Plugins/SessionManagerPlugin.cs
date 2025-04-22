using ChatbotBackend.Repository;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class SessionManagerPlugin
    {
        private readonly UserContext _userContext;
        private readonly IUserRepository _userRepository;
        public SessionManagerPlugin(UserContext userContext, IUserRepository userRepository)
        {
            _userContext = userContext;
            _userRepository = userRepository;
        }

        [KernelFunction]
        [Description("try to signin user based on given username and password")]
        public async Task<bool> Signin(
        [Description("expected username for signin")] string userName,
        [Description("expected password for signin")] string passWord)
        {
            int userId;
            try
            {
                userId = await _userRepository.SignIn(userName, passWord);
                ChatHub._connectionUserMap[_userContext.Id] = userId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [KernelFunction]
        [Description("try to signup user based on given user name and password")]
        public async Task<bool> SignUp(
        [Description("expected username for signUp")] string userName,
        [Description("expected password for SignUp")] string passWord)
        {
            int userId;
            try
            {
                userId = await _userRepository.SignUp(userName, passWord);
                ChatHub._connectionUserMap[_userContext.Id] = userId;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

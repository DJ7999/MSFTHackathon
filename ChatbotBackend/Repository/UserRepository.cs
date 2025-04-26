using ChatbotBackend.EntityFramework;
using ChatbotBackend.Models;
using ChatbotBackend.Models.DbModel;
using ChatbotBackend.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace ChatbotBackend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserContext _userContext;
        // Injecting AppDbContext via constructor for Dependency Injection
        public UserRepository(AppDbContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        private async Task<User> GetUserModel()
        {
            int userId = ChatHub._connectionUserMap[_userContext.Id];
            return _context.Users.Where(g => g.Id == userId).SingleOrDefault();
        }

        // Asynchronous method to get goals for a specific user
        public async Task<UserDto> GetUser()
        {
            var user = await GetUserModel();
            return new UserDto
            {
                MonthlyExpense = user.MonthlyExpense,
                MonthlySalary = user.MonthlySalary,
                Name = user.Name
            };
        }

        public async Task<int> SignIn(string username, string password)
        {
            var user = _context.Users.Where(u =>
            string.Equals(username.ToLower(), u.Name.ToLower())
            && string.Equals(password.ToLower(), u.Password.ToLower())).SingleOrDefault();
            if (user == null)
            {
                throw new Exception("user not found");
            }
            return user.Id;
        }
        public async Task<int> SignUp(string username, string password)
        {
            // Optional: Check if username is already taken
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Name.ToLower() == username.ToLower());

            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            var user = new User
            {
                Name = username,
                Password = password // Consider hashing the password in real apps!
            };

            try
            {
                await _context.Users.AddAsync(user);
                var result = await _context.SaveChangesAsync();

                if (result == 0)
                {
                    throw new Exception("Unable to create account.");
                }

                return user.Id; // Assuming 'Id' is your primary key
            }
            catch (Exception)
            {
                throw; // preserves the original stack trace
            }
        }

        public async Task UpdateUserSalary(float salary)
        {
            var user = await GetUserModel();
            user.MonthlySalary = salary;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public async Task UpdateUserMonthlyExpense(float monthlyExpense)
        {
            var user = await GetUserModel();
            user.MonthlyExpense = monthlyExpense;
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}

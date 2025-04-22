using ChatbotBackend.EntityFramework;
using ChatbotBackend.Models.DbModel;
using ChatbotBackend.Models.DTO;

namespace ChatbotBackend.Repository
{
    public class RetirementRepository : IRetirementRepository
    {
        private readonly AppDbContext _context;
        private readonly UserContext _userContext;
        // Injecting AppDbContext via constructor for Dependency Injection
        public RetirementRepository(AppDbContext context, UserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // Asynchronous method to get goals for a specific user
        private async Task<Retirement?> GetRetirement()
        {
            int userId = ChatHub._connectionUserMap[_userContext.Id];
            return _context.Retirements.Where(g => g.UserId == userId).SingleOrDefault();

        }
        public async Task<RetirementDto> GetRetirementPlan()
        {

            var retirement = await GetRetirement();
            if (retirement == null) { return new RetirementDto(); }
            return new RetirementDto
            {
                FireAmount = retirement.FireAmount,
                InflationAdjustedFireAmount = retirement.InflationAdjustedFireAmount,
                RetirementAge = retirement.RetirementAge,
                SipRequired = retirement.SipRequired
            };
        }

        public async Task AddUpdateRetirementPlan(RetirementDto retirementDto)
        {
            var retirement = await GetRetirement();
            if (retirement == null)
            {
                retirement = new Retirement
                {
                    RetirementAge = retirementDto.RetirementAge,
                    FireAmount = retirementDto.FireAmount,
                    InflationAdjustedFireAmount = retirementDto.InflationAdjustedFireAmount,
                    SipRequired = retirementDto.SipRequired,
                    UserId = ChatHub._connectionUserMap[_userContext.Id]
                };
                await _context.Retirements.AddAsync(retirement);
            }
            else
            {
                retirement.RetirementAge = retirementDto.RetirementAge;
                retirement.FireAmount = retirementDto.FireAmount;
                retirement.InflationAdjustedFireAmount = retirementDto.InflationAdjustedFireAmount;
                retirement.SipRequired = retirementDto.SipRequired;
                _context.Retirements.Update(retirement);
            }
            await _context.SaveChangesAsync(); // Save changes to the database
        }
        public async Task RemoveRetirementPlan()
        {
            var retirementPlan = await GetRetirement();
            if (retirementPlan != null)
                _context.Retirements.Remove(retirementPlan);
            await _context.SaveChangesAsync(); // Save changes to the database
        }
    }
}

namespace ChatbotBackend.Models.DbModel
{
    public class Retirement
    {
        public int RetirementAge { get; set; }
        public decimal FireAmount { get; set; }
        public decimal InflationAdjustedFireAmount { get; set; }
        public decimal SipRequired { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}

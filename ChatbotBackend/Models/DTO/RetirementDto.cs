namespace ChatbotBackend.Models.DTO
{
    public class RetirementDto
    {
        public int RetirementAge { get; set; }
        public decimal FireAmount { get; set; }
        public decimal InflationAdjustedFireAmount { get; set; }
        public decimal SipRequired { get; set; }
    }
}

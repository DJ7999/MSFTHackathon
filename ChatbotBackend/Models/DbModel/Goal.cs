namespace ChatbotBackend.Models.DbModel
{
    public class Goal
    {
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal requiredSip { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}

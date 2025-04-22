namespace ChatbotBackend.Models.DbModel
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public float MonthlySalary { get; set; }
        public float MonthlyExpense { get; set; }

        public List<Portfolio> Portfolios { get; set; }
        public List<Goal> Goals { get; set; }
        public Retirement Retirement { get; set; } // one-to-one
    }
}


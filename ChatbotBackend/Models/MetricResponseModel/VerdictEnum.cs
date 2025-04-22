namespace ChatbotBackend
{
    public enum StockVerdict
    {
        StrongBuy = 5,   // 85+
        Good = 4,        // 75–85
        Watchlist = 3,   // 60–75
        Weak = 2,        // 50–60
        Exit = 1         // <50
    }
}

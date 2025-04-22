namespace ChatbotBackend.Models.MetricModel
{
    public class GrowthMetricModel
    {
        public float Revenue { get; set; }//>10%
        public float Profit { get; set; }//>10%
        public float EarningPerShareCagr { get; set; }//>10%
        public float SectorRevenue { get; set; }
        public float SectorProfit { get; set; }
        public float SectorEarningPerShareCagr { get; set; }
    }
}

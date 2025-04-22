namespace ChatbotBackend.Models.MetricModel
{
    public class LeverageAndSolvencyMetricModel
    {
        public float DebtToEquity { get; set; }//<0.5
        public float InterestCoverage { get; set; }//>3
        public float SectorDebtToEquity { get; set; } = 0.5f;
        public float SectorInterestCoverage { get; set; } = 3f;
    }
}

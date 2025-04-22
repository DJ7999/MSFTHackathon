namespace ChatbotBackend.Models.MetricModel
{
    public class CashFlowHealthMetricModel
    {
        public float OperatingCashFlow { get; set; } // >0
        public float FreeCashFlow { get; set; } // >0
        public float CashConversionRatio { get; set; } // OCF/Net Profit >1
    }
}

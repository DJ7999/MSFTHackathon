using ChatbotBackend.Models.MetricModel;


namespace ChatbotBackend.Models.MetricResponseModel
{
    public class CashFlowHealthResponseModel
    {
        public CashFlowHealthMetricModel CashFlowHealthMetric { get; set; }
        public string Message { get; set; }
    }
}

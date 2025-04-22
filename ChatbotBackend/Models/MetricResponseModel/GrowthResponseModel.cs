using ChatbotBackend.Models.MetricModel;

namespace ChatbotBackend.Models.MetricResponseModel
{
    public class GrowthResponseModel
    {
        public GrowthMetricModel GrowthMetric { get; set; }
        public string Message { get; set; }
    }
}

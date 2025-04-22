using ChatbotBackend.Models.MetricModel;

namespace ChatbotBackend.Models.MetricResponseModel
{
    public class ProfibilityResponseModel
    {
        public ProfibilityMetricModel ProfibilityMetric { get; set; }
        public string Message { get; set; }
    }
}

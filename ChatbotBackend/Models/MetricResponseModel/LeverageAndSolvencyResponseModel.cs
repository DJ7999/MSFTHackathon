using ChatbotBackend.Models.MetricModel;

namespace ChatbotBackend.Models.MetricResponseModel
{
    public class LeverageAndSolvencyResponseModel
    {
        public LeverageAndSolvencyMetricModel LeverageAndSolvencyMetric { get; set; }
        public string Message { get; set; }
    }
}

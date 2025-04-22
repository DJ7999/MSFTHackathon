namespace ChatbotBackend.Models.MetricModel
{
    public class OwnershipAndRedFlagsMetricModel
    {
        public float PromoterHolding { get; set; } // >50%
        public float PledgedSharesPercent { get; set; } // = 0%
        public bool IsFiiDiiHoldingIncreasing { get; set; } // true/false
        public bool HasRedFlags { get; set; } // false = good
    }
}

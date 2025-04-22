namespace ChatbotBackend.Models.MetricModel
{
    public class LiquidityAndEffiiciencyMetricModel
    {
        public float CurrentRatio { get; set; } // >1.5
        public float QuickRatio { get; set; } // >1
        public float InventoryTurnover { get; set; } // >4 or sector avg
        public float ReceivablesTurnover { get; set; } // >6 or sector avg

        public float SectorCurrentRatio { get; set; } = 1.5f;
        public float SectorQuickRatio { get; set; } = 1f;
        public float SectorInventoryTurnover { get; set; } = 4f;
        public float SectorReceivablesTurnover { get; set; } = 6f;
    }
}

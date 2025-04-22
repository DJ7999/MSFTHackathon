namespace ChatbotBackend.Models
{
    public class ValuationMetricModel
    {
        public float PegRatio { get; set; } //<1
        public float PeRatio { get; set; } //sector
        public float PbRatio { get; set; } //<3
        public float Ev_Ebidta { get; set; } //<10
        public float SectorPegRatio { get; set; } = 1;
        public float SectorPeRatio { get; set; }
        public float SectorPbRatio { get; set; } = 3;
        public float SectorEv_Ebidta { get; set; } = 10;
    }
}

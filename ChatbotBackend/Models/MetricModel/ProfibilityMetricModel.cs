namespace ChatbotBackend.Models.MetricModel
{
    public class ProfibilityMetricModel
    {
        public float Roe { get; set; }//>15 or sector
        public float Roce { get; set; }//>15 or sector
        public float NetProfitMargine { get; set; }//>10
        public float EbitdaMargin { get; set; }////>15 or sector
        public float SectorRoe { get; set; } = 15;
        public float SectorRoce { get; set; } = 15;
        public float SectorNetProfitMargine { get; set; } = 10;
        public float SectorEbitdaMargin { get; set; } = 15;
    }
}

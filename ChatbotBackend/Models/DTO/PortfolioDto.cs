using System.ComponentModel.DataAnnotations.Schema;

namespace ChatbotBackend.Models.DTO
{
    public class PortfolioDto
    {
        public string AssetName { get; set; }
        public float Value { get; set; }
        public int Quantity { get; set; }
    }
}

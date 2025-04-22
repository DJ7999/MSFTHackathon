using System.ComponentModel.DataAnnotations.Schema;

namespace ChatbotBackend.Models.DbModel
{
    public class Portfolio
    {
        public string AssetName { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

// Models/OrderItem.cs
namespace TubeShopBackend.Models
{
    public class OrderItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } // "tons" или "meters"
        public decimal PricePerUnit { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
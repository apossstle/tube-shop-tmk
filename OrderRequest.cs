// Models/OrderRequest.cs
namespace TubeShopBackend.Models
{
    public class OrderRequest
    {
        public string OrderId { get; set; }
        public CustomerInfo Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Новый";
    }
}
namespace TubeShopBackend.Models;

public class CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightPerMeter { get; set; }
    public decimal QuantityMeters { get; set; }
    public decimal QuantityTons { get; set; }
    public string Warehouse { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public decimal FinalPrice { get; set; }
}
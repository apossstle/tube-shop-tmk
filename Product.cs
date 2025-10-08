namespace TubeShopBackend.Models;

public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Diameter { get; set; }
    public decimal WallThickness { get; set; }
    public string Standard { get; set; } = string.Empty;
    public string SteelGrade { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightPerMeter { get; set; }
    public int Stock { get; set; }
    public string Warehouse { get; set; } = string.Empty;
}
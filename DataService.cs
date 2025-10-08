using System.Text.Json;
using System.Globalization;
using TubeShopBackend.Models;

namespace TubeShopBackend.Services;

public class DataService
{
    private readonly List<Product> _products = new();

    public DataService()
    {
        LoadProducts();
    }

    private void LoadProducts()
    {
        try
        {
            var pricesData = LoadPrices();
            var nomenclatureData = LoadNomenclature();
            var remnantsData = LoadRemnants();
            var stocksData = LoadStocks();
            var typesData = LoadTypes();

            _products.Clear();
            foreach (var price in pricesData)
            {
                var nomenclature = nomenclatureData.FirstOrDefault(n => n.ID == price.ID);
                var remnant = remnantsData.FirstOrDefault(r => r.ID == price.ID && r.IDStock == price.IDStock);
                var stock = stocksData.FirstOrDefault(s => s.IDStock == price.IDStock);
                var type = typesData.FirstOrDefault(t => t.IDType == nomenclature?.IDType);

                _products.Add(new Product
                {
                    Id = price.ID,
                    Name = nomenclature?.Name ?? $"Товар {price.ID}",
                    Type = type?.Type ?? "Труба",
                    Diameter = ExtractDiameter(nomenclature?.Name),
                    WallThickness = ExtractWallThickness(nomenclature?.Name),
                    Standard = nomenclature?.Gost ?? "ГОСТ",
                    SteelGrade = ExtractSteelGrade(nomenclature?.Name),
                    Price = price.PriceT,
                    WeightPerMeter = CalculateWeightPerMeter(nomenclature?.Name),
                    Stock = (int)(remnant?.InStockM ?? 0),
                    Warehouse = stock?.StockName ?? "Неизвестный склад"
                });
            }
        }
        catch (Exception)
        {
            _products.Clear();
        }
    }

    private List<PriceData> LoadPrices()
    {
        var pricesPath = Path.Combine("Data", "prices.json");
        var pricesJson = File.ReadAllText(pricesPath);

        var options = new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var pricesRoot = JsonSerializer.Deserialize<PricesRoot>(pricesJson, options);
        return pricesRoot?.ArrayOfPricesEl ?? new List<PriceData>();
    }

    private List<NomenclatureData> LoadNomenclature()
    {
        var path = Path.Combine("Data", "nomenclature.json");
        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var root = JsonSerializer.Deserialize<NomenclatureRoot>(json, options);
        return root?.ArrayOfNomenclatureEl ?? new List<NomenclatureData>();
    }

    private List<RemnantsData> LoadRemnants()
    {
        var path = Path.Combine("Data", "remnants.json");
        var json = File.ReadAllText(path);

        var options = new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        var root = JsonSerializer.Deserialize<RemnantsRoot>(json, options);
        return root?.ArrayOfRemnantsEl ?? new List<RemnantsData>();
    }

    private List<StockData> LoadStocks()
    {
        var path = Path.Combine("Data", "stocks.json");
        var json = File.ReadAllText(path);
        var root = JsonSerializer.Deserialize<StocksRoot>(json);
        return root?.ArrayOfStockEl ?? new List<StockData>();
    }

    private List<TypeData> LoadTypes()
    {
        var path = Path.Combine("Data", "types.json");
        var json = File.ReadAllText(path);
        var root = JsonSerializer.Deserialize<TypesRoot>(json);
        return root?.ArrayOfTypeEl ?? new List<TypeData>();
    }

    private decimal ExtractDiameter(string name)
    {
        if (string.IsNullOrEmpty(name)) return 50;
        var match = System.Text.RegularExpressions.Regex.Match(name, @"D\s*(\d+\.?\d*)");
        return match.Success ? decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 50;
    }

    private decimal ExtractWallThickness(string name)
    {
        if (string.IsNullOrEmpty(name)) return 3.5m;
        var match = System.Text.RegularExpressions.Regex.Match(name, @"S\s*(\d+\.?\d*)");
        return match.Success ? decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) : 3.5m;
    }

    private string ExtractSteelGrade(string name)
    {
        if (string.IsNullOrEmpty(name)) return "Ст20";
        var match = System.Text.RegularExpressions.Regex.Match(name, @"МСт\s*([^,]+)");
        return match.Success ? match.Groups[1].Value.Trim() : "Ст20";
    }

    private decimal CalculateWeightPerMeter(string name)
    {
        var diameter = ExtractDiameter(name);
        var wallThickness = ExtractWallThickness(name);
        return diameter * wallThickness * 0.02466m;
    }

    public List<Product> GetProducts() => _products;

    private class PricesRoot { public List<PriceData> ArrayOfPricesEl { get; set; } = new(); }
    private class PriceData
    {
        public string ID { get; set; } = string.Empty;
        public string IDStock { get; set; } = string.Empty;
        public decimal PriceT { get; set; }
    }

    private class NomenclatureRoot { public List<NomenclatureData> ArrayOfNomenclatureEl { get; set; } = new(); }
    private class NomenclatureData { public string ID { get; set; } = string.Empty; public string IDType { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public string Gost { get; set; } = string.Empty; }

    private class RemnantsRoot { public List<RemnantsData> ArrayOfRemnantsEl { get; set; } = new(); }
    private class RemnantsData
    {
        public string ID { get; set; } = string.Empty;
        public string IDStock { get; set; } = string.Empty;
        public decimal InStockM { get; set; }
    }

    private class StocksRoot { public List<StockData> ArrayOfStockEl { get; set; } = new(); }
    private class StockData { public string IDStock { get; set; } = string.Empty; public string StockName { get; set; } = string.Empty; }

    private class TypesRoot { public List<TypeData> ArrayOfTypeEl { get; set; } = new(); }
    private class TypeData { public string IDType { get; set; } = string.Empty; public string Type { get; set; } = string.Empty; }
}
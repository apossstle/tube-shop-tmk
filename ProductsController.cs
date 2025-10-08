using Microsoft.AspNetCore.Mvc;
using TubeShopBackend.Services;

namespace TubeShopBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly DiscountService _discountService;

    public ProductsController(DataService dataService)
    {
        _dataService = dataService;
        _discountService = new DiscountService();
    }

    [HttpGet]
    public IActionResult GetProducts(
        [FromQuery] string? type,
        [FromQuery] string? warehouse,
        [FromQuery] decimal? diameter,
        [FromQuery] decimal? wallThickness,
        [FromQuery] string? steelGrade,
        [FromQuery] string? standard)
    {
        var products = _dataService.GetProducts().AsQueryable();

        // Применяем фильтры
        if (!string.IsNullOrEmpty(type))
            products = products.Where(p => p.Type.Contains(type));

        if (!string.IsNullOrEmpty(warehouse))
            products = products.Where(p => p.Warehouse.Contains(warehouse));

        if (diameter.HasValue)
            products = products.Where(p => p.Diameter == diameter.Value);

        if (wallThickness.HasValue)
            products = products.Where(p => p.WallThickness == wallThickness.Value);

        if (!string.IsNullOrEmpty(steelGrade))
            products = products.Where(p => p.SteelGrade.Contains(steelGrade));

        if (!string.IsNullOrEmpty(standard))
            products = products.Where(p => p.Standard.Contains(standard));

        return Ok(products.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetProduct(string id)
    {
        var product = _dataService.GetProducts().FirstOrDefault(p => p.Id == id);
        if (product == null)
            return NotFound($"Товар с ID {id} не найден");

        return Ok(product);
    }

    // НОВЫЙ ENDPOINT ДЛЯ РАСЧЕТА СКИДОК
    [HttpPost("calculate-discount")]
    public IActionResult CalculateDiscount([FromBody] DiscountRequest request)
    {
        try
        {
            var (discountPercent, discountAmount) = _discountService.ApplyDiscount(
                request.TotalPrice,
                request.TotalTons
            );

            var nextDiscount = _discountService.GetNextDiscount(request.TotalTons);

            return Ok(new DiscountResponse
            {
                DiscountPercent = discountPercent,
                DiscountAmount = discountAmount,
                FinalPrice = request.TotalPrice - discountAmount,
                NextDiscount = nextDiscount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // НОВЫЙ ENDPOINT ДЛЯ ПОЛУЧЕНИЯ ПРАВИЛ СКИДОК
    [HttpGet("discount-rules")]
    public IActionResult GetDiscountRules()
    {
        var rules = _discountService.GetDiscountRules();
        return Ok(rules);
    }
}

// МОДЕЛИ ДЛЯ СКИДОК (оставляем в этом файле)
public class DiscountRequest
{
    public decimal TotalPrice { get; set; }
    public decimal TotalTons { get; set; }
}

public class DiscountResponse
{
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
    public NextDiscountInfo? NextDiscount { get; set; }
}
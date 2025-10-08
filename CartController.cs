using Microsoft.AspNetCore.Mvc;
using TubeShopBackend.Models;
using TubeShopBackend.Services;

namespace TubeShopBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly DataService _dataService;

    public CartController(CartService cartService, DataService dataService)
    {
        _cartService = cartService;
        _dataService = dataService;
    }

    [HttpPost("add")]
    public IActionResult AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            Console.WriteLine($"🛒 Добавление в корзину: ID={request.ProductId}, Meters={request.QuantityMeters}, Tons={request.QuantityTons}");

            var product = _dataService.GetProducts().FirstOrDefault(p => p.Id == request.ProductId);
            if (product == null)
            {
                Console.WriteLine("❌ Товар не найден");
                return NotFound("Товар не найден");
            }

            Console.WriteLine($"✅ Товар найден: {product.Name}");

            decimal quantityMeters = request.QuantityMeters;
            decimal quantityTons = request.QuantityTons;

            // Автоматическая конвертация если одно значение 0
            if (quantityMeters > 0 && quantityTons == 0)
            {
                quantityTons = _cartService.ConvertMetersToTons(quantityMeters, product.WeightPerMeter);
                Console.WriteLine($"📏 Конвертация: {quantityMeters}м → {quantityTons}т");
            }
            else if (quantityTons > 0 && quantityMeters == 0)
            {
                quantityMeters = _cartService.ConvertTonsToMeters(quantityTons, product.WeightPerMeter);
                Console.WriteLine($"⚖️ Конвертация: {quantityTons}т → {quantityMeters}м");
            }

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                WeightPerMeter = product.WeightPerMeter,
                Warehouse = product.Warehouse,
                QuantityMeters = quantityMeters, 
                QuantityTons = quantityTons 
            };

            _cartService.AddToCart(cartItem);
            Console.WriteLine($"✅ Товар добавлен в корзину. Всего в корзине: {_cartService.GetCart().Count}");

            return Ok(new { message = "Товар добавлен в корзину" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка в AddToCart: {ex.Message}");
            Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            return StatusCode(500, new { error = "Внутренняя ошибка сервера" });
        }
    }
    [HttpGet]
    [HttpGet]
    public IActionResult GetCart()
    {
        var cartSummary = _cartService.GetCartWithDiscounts();
        return Ok(cartSummary);
    }

    [HttpDelete("{productId}")]
    public IActionResult RemoveFromCart(string productId)
    {
        _cartService.RemoveFromCart(productId);
        return Ok(new { message = "Товар удален из корзины" });
    }

    [HttpPost("clear")]
    public IActionResult ClearCart()
    {
        try
        {
            _cartService.ClearCart();
            Console.WriteLine("✅ Корзина очищена");
            return Ok(new { message = "Корзина очищена" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при очистке корзины: {ex.Message}");
            return StatusCode(500, new { error = "Ошибка при очистке корзины" });
        }
    }

}

public class AddToCartRequest
{
    public string ProductId { get; set; } = string.Empty;
    public decimal QuantityMeters { get; set; }
    public decimal QuantityTons { get; set; }
}
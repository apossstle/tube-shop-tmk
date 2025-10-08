using TubeShopBackend.Models;

namespace TubeShopBackend.Services;

public class CartService
{
    private readonly List<CartItem> _cartItems = new();

    public void AddToCart(CartItem item)
    {
        var existingItem = _cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);
        if (existingItem != null)
        {
            // ���� ����� ��� � ������� - ��������� ����������
            existingItem.QuantityMeters += item.QuantityMeters;
            existingItem.QuantityTons += item.QuantityTons;
        }
        else
        {
            _cartItems.Add(item);
        }
    }

    public void RemoveFromCart(string productId)
    {
        var item = _cartItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            _cartItems.Remove(item);
        }
    }

    public List<CartItem> GetCart()
    {
        return _cartItems;
    }

    public void ClearCart()
    {
        _cartItems.Clear();
    }

    // ����������� ������ � �����
    public decimal ConvertMetersToTons(decimal meters, decimal weightPerMeter)
    {
        return meters * weightPerMeter / 1000;
    }

    // ����������� ���� � �����
    public decimal ConvertTonsToMeters(decimal tons, decimal weightPerMeter)
    {
        return weightPerMeter > 0 ? (tons * 1000) / weightPerMeter : 0;
    }
}
// ? ������ � ����� ����� CartService.cs
public class CartSummary
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalTons { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
}

// ? ������ ������ � ����� CartService
public decimal CalculateItemDiscount(decimal quantityTons)
    {
        if (quantityTons >= 50) return 7m;    // 7%
        if (quantityTons >= 20) return 5m;    // 5%
        if (quantityTons >= 10) return 3m;    // 3%
        return 0m;
    }

    public CartSummary GetCartWithDiscounts()
    {
        var cartItems = GetCart();
        decimal totalTons = 0;
        decimal totalPrice = 0;

        foreach (var item in cartItems)
        {
            totalTons += item.QuantityTons;
            totalPrice += item.Price * item.QuantityTons;

            // ��������� ������ ��� ������� ������
            item.DiscountPercent = CalculateItemDiscount(item.QuantityTons);
            item.FinalPrice = item.Price * item.QuantityTons * (1 - item.DiscountPercent / 100);
        }

        var discountPercent = CalculateItemDiscount(totalTons);
        var discountAmount = totalPrice * discountPercent / 100;

        return new CartSummary
        {
            Items = cartItems,
            TotalTons = totalTons,
            TotalPrice = totalPrice,
            DiscountPercent = discountPercent,
            DiscountAmount = discountAmount,
            FinalPrice = totalPrice - discountAmount
        };
    }
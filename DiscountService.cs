namespace TubeShopBackend.Services;

public class DiscountService
{
    private readonly List<DiscountRule> _discountRules = new()
    {
        new DiscountRule { MinTons = 10m, DiscountPercent = 3m },
        new DiscountRule { MinTons = 50m, DiscountPercent = 7m },
        new DiscountRule { MinTons = 100m, DiscountPercent = 10m },
        new DiscountRule { MinTons = 200m, DiscountPercent = 15m }
    };

    public decimal CalculateDiscount(decimal totalTons)
    {
        var applicableRule = _discountRules
            .Where(rule => totalTons >= rule.MinTons)
            .OrderByDescending(rule => rule.MinTons)
            .FirstOrDefault();

        return applicableRule?.DiscountPercent ?? 0m;
    }

    public (decimal discountPercent, decimal discountAmount) ApplyDiscount(decimal totalPrice, decimal totalTons)
    {
        var discountPercent = CalculateDiscount(totalTons);
        var discountAmount = totalPrice * (discountPercent / 100m);
        return (discountPercent, discountAmount);
    }

    public NextDiscountInfo? GetNextDiscount(decimal currentTons)
    {
        var nextRule = _discountRules
            .Where(rule => rule.MinTons > currentTons)
            .OrderBy(rule => rule.MinTons)
            .FirstOrDefault();

        if (nextRule != null)
        {
            return new NextDiscountInfo
            {
                TonsNeeded = nextRule.MinTons - currentTons,
                DiscountPercent = nextRule.DiscountPercent
            };
        }

        return null;
    }

    public List<DiscountRule> GetDiscountRules()
    {
        return _discountRules.OrderBy(rule => rule.MinTons).ToList();
    }
}

public class DiscountRule
{
    public decimal MinTons { get; set; }
    public decimal DiscountPercent { get; set; }
}

public class NextDiscountInfo
{
    public decimal TonsNeeded { get; set; }
    public decimal DiscountPercent { get; set; }
}
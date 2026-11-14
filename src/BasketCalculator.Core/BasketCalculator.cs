namespace BasketCalculator.Core;

using System;
using System.Collections.Generic;

public static class BasketCalculator
{
    private static readonly Dictionary<string, long> Prices = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Apple"] = 35L,
        ["Banana"] = 20L,
        ["Melon"] = 50L,
        ["Lime"] = 15L,
    };

    public static long Calculate(string[]? basket)
    {
        if (basket is null)
        {
            throw new ArgumentNullException(nameof(basket));
        }

        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in basket)
        {
            if (item is null)
            {
                throw new ArgumentException("Basket items cannot be null.", nameof(basket));
            }

            var normalized = item.Trim();
            if (normalized.Length == 0)
            {
                throw new ArgumentException("Basket items cannot be empty or whitespace.", nameof(basket));
            }

            if (!Prices.ContainsKey(normalized))
            {
                throw new ArgumentException($"Unknown item '{item}'.", nameof(basket));
            }

            counts[normalized] = counts.TryGetValue(normalized, out var count) ? count + 1 : 1;
        }

        long total = 0;

        foreach (var (item, quantity) in counts)
        {
            if (item.Equals("Melon", StringComparison.OrdinalIgnoreCase))
            {
                total += (quantity - quantity / 2) * Prices[item];
            }
            else if (item.Equals("Lime", StringComparison.OrdinalIgnoreCase))
            {
                total += (quantity - quantity / 3) * Prices[item];
            }
            else
            {
                total += quantity * Prices[item];
            }
        }

        return total;
    }
}

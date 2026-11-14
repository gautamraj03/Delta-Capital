using BasketCalculatorService = BasketCalculator.Core.BasketCalculator;
using Xunit;

namespace BasketCalculator.Tests;

public class BasketCalculatorTests
{
    [Fact]
    public void EmptyBasket_CostsZero()
    {
        Assert.Equal(0L, BasketCalculatorService.Calculate(Array.Empty<string>()));
    }

    [Theory]
    [InlineData("Apple", 35L)]
    [InlineData("Banana", 20L)]
    [InlineData("Melon", 50L)]
    [InlineData("Lime", 15L)]
    public void SingleItem_CostsItsListedPrice(string item, long expectedPence)
    {
        Assert.Equal(expectedPence, BasketCalculatorService.Calculate(new[] { item }));
    }

    [Theory]
    [MemberData(nameof(MelonCases))]
    public void Melons_ApplyBuyOneGetOneFree(int quantity, long expectedPence)
    {
        Assert.Equal(expectedPence, BasketCalculatorService.Calculate(Repeat("Melon", quantity)));
    }

    public static IEnumerable<object[]> MelonCases()
    {
        for (var quantity = 0; quantity <= 12; quantity++)
        {
            yield return new object[] { quantity, ((quantity + 1) / 2) * 50L };
        }
    }

    [Theory]
    [MemberData(nameof(LimeCases))]
    public void Limes_ApplyThreeForThePriceOfTwo(int quantity, long expectedPence)
    {
        Assert.Equal(expectedPence, BasketCalculatorService.Calculate(Repeat("Lime", quantity)));
    }

    public static IEnumerable<object[]> LimeCases()
    {
        for (var quantity = 0; quantity <= 12; quantity++)
        {
            yield return new object[] { quantity, (quantity - quantity / 3) * 15L };
        }
    }

    [Theory]
    [MemberData(nameof(MixedBasketCases))]
    public void MixedBasket_CalculatesTotal(string[] items, long expectedPence)
    {
        Assert.Equal(expectedPence, BasketCalculatorService.Calculate(items));
    }

    public static IEnumerable<object[]> MixedBasketCases()
    {
        yield return new object[]
        {
            new[] { "Apple", "Banana", "Melon", "Melon", "Lime", "Lime", "Lime" },
            135L
        };

        yield return new object[]
        {
            new[]
            {
                "Apple", "Apple",
                "Banana", "Banana", "Banana",
                "Melon", "Melon", "Melon",
                "Lime", "Lime", "Lime", "Lime"
            },
            275L
        };
    }

    [Theory]
    [InlineData("apple", 35L)]
    [InlineData("APPLE", 35L)]
    [InlineData("  Apple  ", 35L)]
    [InlineData("banana", 20L)]
    [InlineData("  MELON ", 50L)]
    [InlineData(" lime  ", 15L)]
    public void ItemNames_AreTrimmedAndCaseInsensitive(string item, long expectedPence)
    {
        Assert.Equal(expectedPence, BasketCalculatorService.Calculate(new[] { item }));
    }

    [Fact]
    public void Total_DoesNotDependOnItemOrder()
    {
        var firstOrder = new[]
        {
            "Melon", "Lime", "Apple", "Melon", "Lime", "Banana", "Lime"
        };
        var anotherOrder = new[]
        {
            "Banana", "Lime", "Melon", "Apple", "Lime", "Melon", "Lime"
        };

        Assert.Equal(135L, BasketCalculatorService.Calculate(firstOrder));
        Assert.Equal(
            BasketCalculatorService.Calculate(firstOrder),
            BasketCalculatorService.Calculate(anotherOrder));
    }

    [Fact]
    public void NullBasket_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => BasketCalculatorService.Calculate(null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("Pear")]
    [InlineData("Appl")]
    public void NullBlankOrUnknownItem_ThrowsArgumentException(string? item)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => BasketCalculatorService.Calculate(new[] { item! }));
    }

    [Fact]
    public void InvalidItemInBasket_ThrowsInsteadOfReturningPartialTotal()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => BasketCalculatorService.Calculate(
                new[] { "Apple", "UnknownItem", "Banana" }));
    }

    private static string[] Repeat(string item, int quantity)
    {
        var items = new string[quantity];
        Array.Fill(items, item);
        return items;
    }
}

using BasketCalculatorService = BasketCalculator.Core.BasketCalculator;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: dotnet run --project src/BasketCalculator.Cli -- <item> [item ...]");
    Console.Error.WriteLine("Items: Apple, Banana, Melon, Lime");
    return 1;
}

try
{
    var totalPence = BasketCalculatorService.Calculate(args);
    Console.WriteLine($"Total: {totalPence}p");
    return 0;
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine($"Error: {exception.Message}");
    return 1;
}

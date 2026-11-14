# Basket Calculator

A small .NET command-line application that totals a shopping basket in pence. It supports regular prices, two promotional offers, input validation, and an xUnit test suite.

## Requirements

- .NET 11 SDK

## Run

From the repository root:

```bash
dotnet run --project src/BasketCalculator.Cli -- Apple Banana Melon Melon Lime Lime Lime
```

Output:

```text
Total: 135p
```

Item names are case-insensitive and surrounding whitespace is ignored. Supported items are `Apple`, `Banana`, `Melon`, and `Lime`. Running without items or including an unknown item prints an error to standard error and returns a non-zero exit code.

## Prices and offers

| Item | Unit price | Offer |
| --- | ---: | --- |
| Apple | 35p | None |
| Banana | 20p | None |
| Melon | 50p | Buy one, get one free |
| Lime | 15p | Three for the price of two |

## Test

```bash
dotnet test BasketCalculator.sln
```

The test suite covers listed prices, promotional quantities, mixed baskets, case/whitespace normalization, order independence, and invalid input.

## Solution structure

```text
src/
  BasketCalculator.Core/   Pricing, validation, and basket calculation
  BasketCalculator.Cli/    Command-line entry point
tests/
  BasketCalculator.Tests/  xUnit tests for the core calculation
docs/
  design.md                Architecture and behavior details
```

For component responsibilities, calculation rules, validation behavior, and design decisions, see [docs/design.md](docs/design.md).

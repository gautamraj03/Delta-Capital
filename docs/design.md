# Basket Calculator: Technical Design

## 1. Purpose and scope

Basket Calculator is a small, deterministic application for calculating the total price of a basket containing a fixed catalog of grocery items. It provides both a reusable calculation API and a command-line executable.

### Goals

- Calculate totals in integer pence to avoid floating-point currency errors.
- Apply the supported item-level promotions consistently for any quantity.
- Reject invalid input instead of returning a misleading partial total.
- Keep calculation logic independently testable and reusable.
- Make the project straightforward to build, run, and extend.

### Out of scope

- Persistent storage, user accounts, or network services.
- Configurable catalogs, currencies, tax, discounts, or promotion dates.
- Interactive prompts or reading basket items from a file.
- Product identifiers beyond the four named items.

## 2. Technology

- C# and .NET 11
- .NET SDK-style projects and a solution file
- xUnit for automated tests
- GitHub Actions for build/test validation on pushes and pull requests

## 3. Architecture

The solution separates domain behavior, the user-facing entry point, and verification:

```text
BasketCalculator.Cli  --->  BasketCalculator.Core
BasketCalculator.Tests ---> BasketCalculator.Core
```

### `BasketCalculator.Core`

Contains the static `BasketCalculator.Calculate(string[]? basket)` API. It owns the catalog, input normalization and validation, quantity aggregation, promotion calculations, and total calculation. It has no console or filesystem dependencies.

### `BasketCalculator.Cli`

Accepts item names from command-line arguments, passes them to the core API, and prints the total in pence. Invalid invocation or invalid basket data produces a diagnostic on standard error and an exit code of `1`; successful calculations print to standard output and exit with code `0`.

### `BasketCalculator.Tests`

Contains focused xUnit tests referencing the core project. The CLI remains thin, while the calculation rules can be tested without launching a process.

## 4. Catalog and promotion rules

| Canonical item | Price per charged unit | Offer |
| --- | ---: | --- |
| Apple | 35p | None |
| Banana | 20p | None |
| Melon | 50p | Buy one, get one free |
| Lime | 15p | Three for the price of two |

For quantity `q`:

- Apple cost: `q * 35`
- Banana cost: `q * 20`
- Melon cost: `(q - floor(q / 2)) * 50`, equivalently `ceil(q / 2) * 50`
- Lime cost: `(q - floor(q / 3)) * 15`

The discount applies independently per product type. The total is the sum of the four product subtotals. For example, 3 melons cost 100p and 4 limes cost 45p.

## 5. Calculation flow

1. Reject a null basket.
2. Visit every item. Reject null, empty, whitespace-only, and unknown names.
3. Trim surrounding whitespace. Compare names without regard to case.
4. Count each normalized product.
5. Apply the relevant item pricing/promotion formula to each count.
6. Sum subtotals into a `long` total, measured in pence.
7. Return the total only after all basket items have been validated.

Validation occurs before a total is returned. An invalid item therefore fails the complete operation; no partial amount is exposed.

## 6. Input and error behavior

| Input condition | Core API behavior | CLI behavior |
| --- | --- | --- |
| Empty array | Returns `0` | Not applicable: no command-line items is treated as usage error |
| Null basket | Throws `ArgumentNullException` | Not constructible from ordinary CLI arguments |
| Null, blank, or unknown item | Throws `ArgumentException` | Prints an error and exits `1` |
| Valid basket | Returns total pence | Prints `Total: <amount>p` and exits `0` |

Case is ignored and outer whitespace is trimmed. Internal whitespace is not rewritten, so an item such as `Green Apple` remains unknown.

## 7. Data structures and complexity

- A case-insensitive dictionary stores the catalog prices.
- A second case-insensitive dictionary aggregates quantities.
- Time complexity is `O(n + k)` expected, where `n` is the number of basket entries and `k` is the number of distinct products (at most four).
- Additional memory is `O(k)` for quantity counts.

The public API uses `string[]` to match the command-line argument shape and the original test design. It accepts a nullable array so callers receive an explicit argument exception for null input.

## 8. Testing and quality checks

Run all tests from the solution root:

```bash
dotnet test BasketCalculator.sln --configuration Release
```

The test cases cover:

- Empty basket and individual catalog prices.
- Melon and lime promotion quantities from 0 through 12.
- Representative mixed baskets and order independence.
- Case-insensitive matching and whitespace trimming.
- Null basket, null/blank/unknown item, and invalid item mixed with otherwise valid entries.

GitHub Actions runs the solution test command on pushes and pull requests.

## 9. Build and execution

Build:

```bash
dotnet build BasketCalculator.sln --configuration Release
```

Run:

```bash
dotnet run --project src/BasketCalculator.Cli -- Apple Banana Melon Melon Lime Lime Lime
```

The sample basket costs 135p: Apple 35p + Banana 20p + two charged melons 50p + two charged limes 30p.

## 10. Design decisions and extension points

- **Integer pence:** exact arithmetic is appropriate for the fixed GBP price list and avoids decimal rounding.
- **Core/CLI separation:** calculation can be consumed by another application without depending on terminal behavior.
- **Explicit catalog:** the current fixed catalog is simple to understand. If catalog or offers become configurable, represent them as injected catalog/promotion policies rather than adding unrelated branches to the CLI.
- **Product-specific offers:** current offers are quantity-based within each product. Cross-product bundles or time-dependent offers would need a more general promotion model.
- **No persistence:** each process calculates only the basket it receives; no state is retained between invocations.

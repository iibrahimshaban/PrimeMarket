namespace PrimeMarket.Contracts.PromoCodes;

public record OrderItemSummary(
    string ProductName,
    int Quantity,
    decimal UnitPrice
);

namespace PrimeMarket.Contracts.PromoCodes;

public record OrderSellerSummary(
    string SellerName,
    string BrandName,
    ICollection<OrderItemSummary> Items
);

namespace PrimeMarket.Contracts.PromoCodes;

public record PromoCodeOrderResponse(
    int OrderId,
    string CustomerName,
    string CustomerEmail,
    decimal TotalAmount,
    decimal DiscountAmount,
    string Status,
    string PaymentMethod,
    DateTime OrderDate,
    ICollection<OrderSellerSummary> Sellers
);

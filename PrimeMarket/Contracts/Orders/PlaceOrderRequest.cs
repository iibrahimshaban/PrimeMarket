namespace PrimeMarket.Contracts.Orders;

public record PlaceOrderRequest(
    int AddressId,
    PaymentType PaymentMethod,
    string? PromoCode
);

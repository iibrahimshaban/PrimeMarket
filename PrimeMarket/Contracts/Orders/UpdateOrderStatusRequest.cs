namespace PrimeMarket.Contracts.Orders
{
    public record UpdateOrderStatusRequest
    {
       public OrderStatus Status { get; init; }
    }
}

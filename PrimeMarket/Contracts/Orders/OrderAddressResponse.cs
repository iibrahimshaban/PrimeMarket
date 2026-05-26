namespace PrimeMarket.Contracts.Orders
{
    public record OrderAddressResponse(
        string Street,
        string City,
        string Country
    );
}

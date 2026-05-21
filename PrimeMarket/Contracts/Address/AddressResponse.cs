namespace PrimeMarket.Contracts.Address;

public record AddressResponse(
    int Id,
    string Street,
    string City,
    string Country,
    bool IsDefault
);

namespace PrimeMarket.Contracts.Address;

public record AddAddressRequest(
    string Street,
    string City,
    string Country,
    bool IsDefault = false
);

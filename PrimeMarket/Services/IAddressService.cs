using PrimeMarket.Contracts.Address;

namespace PrimeMarket.Services;

public interface IAddressService
{
    Task<Result<IReadOnlyList<AddressResponse>>> GetUserAddressesAsync(string userId);
    Task<Result<AddressResponse>> AddAddressAsync(string userId, AddAddressRequest request);
}

using PrimeMarket.Contracts.Address;

namespace PrimeMarket.Services;

public class AddressService(ApplicationDbContext context) : IAddressService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Result<IReadOnlyList<AddressResponse>>> GetUserAddressesAsync(string userId)
    {
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ToListAsync();

        return Result.Success(addresses.Adapt<IReadOnlyList<AddressResponse>>());
    }

    public async Task<Result<AddressResponse>> AddAddressAsync(string userId, AddAddressRequest request)
    {
        // if new address is default, unset others
        if (request.IsDefault)
        {
            var existing = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            existing.ForEach(a => a.IsDefault = false);
        }

        var address = new Address
        {
            UserId = userId,
            Street = request.Street,
            City = request.City,
            Country = request.Country,
            IsDefault = request.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Result.Success(address.Adapt<AddressResponse>());
    }
}

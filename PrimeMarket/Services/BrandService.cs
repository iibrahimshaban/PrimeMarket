using PrimeMarket.Contracts.Brand;


namespace PrimeMarket.Services;

public class BrandService(ApplicationDbContext context) : IBrandService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<BrandResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Brands
            .Where(b => b.IsActive)
            .Select(b => new BrandResponse(
                b.Id,
                b.BrandName,
                b.Description,
                b.LogoUrl,
                b.IsActive,
                b.IsVerified,
                b.City,
                b.Country,
                b.User.Products.SelectMany(p => p.Reviews).Any()
                    ? b.User.Products.SelectMany(p => p.Reviews).Average(r => r.Rating)
                    : 0,
                b.User.Products.Count(p => p.IsActive)
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<BrandDetailsResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var brandExists = await _context.Brands.AnyAsync(b => b.Id == id, cancellationToken);
        if (!brandExists)
            return Result.Failure<BrandDetailsResponse>(BrandErrors.BrandNotFound);

        var brand = await _context.Brands
            .Where(b => b.Id == id)
            .Select(b => new BrandDetailsResponse(
                b.Id,
                b.BrandName,
                b.Description,
                b.LogoUrl,
                b.IsActive,
                b.IsVerified,
                b.Street,
                b.City,
                b.Country,
                b.Latitude, 
                b.Longitude,
                b.User.FirstName + " " + b.User.LastName,
                b.User.Products.SelectMany(p => p.Reviews).Any()
                    ? b.User.Products.SelectMany(p => p.Reviews).Average(r => r.Rating)
                    : 0,
                b.User.Products.SelectMany(p => p.Reviews).Count(),
                b.User.Products
                    .Where(p => p.IsActive)
                    .Select(p => new BrandProductResponse(
                        p.Id,
                        p.Name,
                        p.BrandName,
                        p.Price,
                        p.Stock,
                        p.IsActive,
                        p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault(),
                        p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                        p.Reviews.Count()
                    )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return Result.Success(brand!);
    }
}

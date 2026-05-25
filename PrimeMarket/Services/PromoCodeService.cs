using PrimeMarket.Contracts.PromoCodes;
using PrimeMarket.Errors;

namespace PrimeMarket.Services;

public class PromoCodeService(ApplicationDbContext context) : IPromoCodeService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> CreateAsync(CreatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        var codeExists = await _context.PromoCodes.AnyAsync(p => p.Code == request.Code, cancellationToken);
        if (codeExists)
            return Result.Failure(PromoCodeErrors.CodeAlreadyExsist);

        var promoCode = new PromoCode
        {
            Code = request.Code,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            UsageLimit = request.UsageLimit,
            ExpiresAt = request.ExpiresAt
        };

        await _context.PromoCodes.AddAsync(promoCode, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<List<PromoCodeResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.PromoCodes
            .Select(p => new PromoCodeResponse(
                p.Id,
                p.Code,
                p.DiscountType.ToString(),
                p.DiscountValue,
                p.UsageLimit,
                p.UsedCount,
                p.ExpiresAt,
                p.IsActive,
                p.CreatedBy.FirstName + " " + p.CreatedBy.LastName,
                p.CreatedOn
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<PromoCodeDetailsResponse>> GetOrdersByPromoCodeAsync(int promoCodeId, CancellationToken cancellationToken)
    {
        var codeExists = await _context.PromoCodes.AnyAsync(p => p.Id == promoCodeId, cancellationToken);
        if (!codeExists) 
            return Result.Failure<PromoCodeDetailsResponse>(PromoCodeErrors.CodeNotFound);

        var response = await _context.PromoCodes
            .Where(p => p.Id == promoCodeId)
            .Select(p => new PromoCodeDetailsResponse(
                p.Id,
                p.Code,
                p.DiscountType.ToString(),
                p.DiscountValue,
                p.UsageLimit,
                p.UsedCount,
                p.ExpiresAt,
                p.IsActive,
                p.CreatedBy.FirstName + " " + p.CreatedBy.LastName,
                p.CreatedOn,
                p.Orders.Select(o => new PromoCodeOrderResponse(
                    o.Id,
                    o.User.FirstName + " " + o.User.LastName,
                    o.User.Email!,
                    o.TotalAmount,
                    o.DiscountAmount,
                    o.Status.ToString(),
                    o.PaymentMethod.ToString(),
                    o.CreatedOn,
                    o.Items
                        .GroupBy(i => i.Product.SellerId)
                        .Select(g => new OrderSellerSummary(
                            g.First().Product.Seller.FirstName + " " + g.First().Product.Seller.LastName,
                            g.First().Product.BrandName ?? "N/A",
                            g.Select(i => new OrderItemSummary(
                                i.Product.Name,
                                i.Quantity,
                                i.UnitPrice
                            )).ToList()
                        )).ToList()
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return Result.Success(response!);
    }

    public async Task<Result> UpdateAsync(int id, UpdatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        var promoCode = await _context.PromoCodes.FindAsync([id], cancellationToken);
        if (promoCode is null)
            return Result.Failure(PromoCodeErrors.CodeNotFound);

        if (request.Code is not null) promoCode.Code = request.Code;
        if (request.DiscountType is not null) promoCode.DiscountType = request.DiscountType.Value;
        if (request.DiscountValue is not null) promoCode.DiscountValue = request.DiscountValue.Value;
        if (request.UsageLimit is not null) promoCode.UsageLimit = request.UsageLimit.Value;
        if (request.ExpiresAt is not null) promoCode.ExpiresAt = request.ExpiresAt.Value;
        if (request.IsActive is not null) promoCode.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

using PrimeMarket.Contracts.Orders;

namespace PrimeMarket.Helpers
{
    public class OrderExtension
    {
        public static IQueryable<SellerOrderResponse> MapSellerOrders(IQueryable<Order> query, string sellerId)
        {
            return query.Select(o => new SellerOrderResponse(
                o.Id,
                $"{o.User.FirstName} {o.User.LastName}",
                o.User.Email ?? string.Empty,
                o.CreatedOn,
                o.Status,
                o.Items
                    .Where(i => i.Product.SellerId == sellerId)
                    .Sum(i => i.UnitPrice * i.Quantity),
                o.Items
                    .Where(i => i.Product.SellerId == sellerId)
                    .Select(i => new SellerOrderItemResponse(
                        i.ProductId,
                        i.Product.Name,
                        i.Product.Images
                            .Where(img => img.IsPrimary)
                            .Select(img => img.Url)
                            .FirstOrDefault() ?? string.Empty,
                        i.Quantity,
                        i.UnitPrice,
                        i.UnitPrice * i.Quantity
                    ))
                    .ToList()
            ));
        }
    }
}

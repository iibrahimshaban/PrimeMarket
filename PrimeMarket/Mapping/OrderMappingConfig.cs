using PrimeMarket.Contracts.Orders;

namespace PrimeMarket.Mapping;
public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, AdminOrderResponse>()
           .ConstructUsing(o => new AdminOrderResponse(
               o.Id,
               o.User.UserName!,
               o.User.Email ?? string.Empty,
               o.CreatedOn,
               o.Status,
               o.Items
                   .Sum(i => i.UnitPrice * i.Quantity),
               o.PaymentMethod.ToString(),
               new OrderAddressResponse(
                   o.Address.Street,
                   o.Address.City,
                   o.Address.Country
               ),
               o.Items
                   .Select(i => new AdminOrderItemResponse(
                       i.ProductId,
                       i.Product.Name,
                       i.Product.Images
                           .Where(img => img.IsPrimary)
                           .Select(img => img.Url)
                           .FirstOrDefault() ?? string.Empty,
                       i.Quantity,
                       i.UnitPrice,
                       i.UnitPrice * i.Quantity,
                       i.Product.Seller.UserName!
                   ))
                   .ToList()
           ));

        config.NewConfig<Order,CustomerOrderResponse>()
            .ConstructUsing(o => new CustomerOrderResponse(
                o.Id,
                o.CreatedOn,
                o.Status,
                o.Items
                    .Sum(i => i.UnitPrice * i.Quantity),
                o.PaymentMethod.ToString(),
                new OrderAddressResponse(
                    o.Address.Street,
                    o.Address.City,
                    o.Address.Country
                ),
                o.Items
                    .Select(i => new CustomerOrderItemResponse(
                        i.ProductId,
                        i.Product.Name,
                        i.Product.Images
                            .Where(img => img.IsPrimary)
                            .Select(img => img.Url)
                            .FirstOrDefault() ?? string.Empty,
                        i.Quantity,
                        i.UnitPrice,
                        i.UnitPrice * i.Quantity,
                        i.Product.Seller.UserName!
                    ))
                    .ToList()
            ));
    }
}

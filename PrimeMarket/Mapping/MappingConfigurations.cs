using PrimeMarket.Contracts.Authentication;
using PrimeMarket.Contracts.Orders;
using PrimeMarket.Contracts.Products;
using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {

        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.EmailConfirmed, src => true);

        config.NewConfig<UpdateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());

        config.NewConfig<ApplicationUser, AuthResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.ProfilePictureUrl, src => src.ProfilePictureUrl);

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
    }
}

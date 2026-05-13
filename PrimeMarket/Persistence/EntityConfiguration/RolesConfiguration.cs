using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrimeMarket.Abstraction.Const;

namespace PrimeMarket.Persistence.EntityConfiguration;

public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
        [
            new IdentityRole
            {
                Id = DefaultRoles.AdminRoleId,
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper()
            },
            new IdentityRole
             {
                Id = DefaultRoles.CustomerRoleId,
                Name = DefaultRoles.Customer,
                ConcurrencyStamp= DefaultRoles.CustomerRoleConcurrencyStamp,
                NormalizedName= DefaultRoles.Customer.ToUpper(),
            },  
            new IdentityRole
             {
                Id = DefaultRoles.SellerRoleId,
                Name = DefaultRoles.Seller,
                ConcurrencyStamp= DefaultRoles.SellerRoleConcurrencyStamp,
                NormalizedName= DefaultRoles.Seller.ToUpper(),
            }
        ]);
    }
}

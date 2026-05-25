namespace PrimeMarket.Persistence.EntityConfiguration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);

        builder.Property(x => x.LastName).HasMaxLength(100);

        builder.Property(x => x.ProfilePictureUrl).HasMaxLength(300);

        builder.OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.UserId,
            Email = DefaultUsers.Email,
            NormalizedEmail = DefaultUsers.Email.ToUpper(),
            PasswordHash = DefaultUsers.HashedPassword,
            EmailConfirmed = true,
            LockoutEnabled = true,
            SecurityStamp = DefaultUsers.SecurityStamp,
            ConcurrencyStamp = DefaultUsers.ConcurrencyStamp,
            FirstName = DefaultUsers.FirstName,
            LastName = DefaultUsers.LastName,
            UserName = DefaultUsers.UserName,
            NormalizedUserName = DefaultUsers.UserName.ToUpper()
        });
    }
}

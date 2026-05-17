using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimeMarket.Authentication;
using PrimeMarket.Entities;
using PrimeMarket.Persistence;
using PrimeMarket.Services;
using PrimeMarket.Services.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PrimeMarket;

public static class DependancyInjection
{
    public static IServiceCollection AddDependancies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddDbContextConfiguration(configuration)
            .AddCloudinaryImageHosting(configuration)
            .AddAuthConfig(configuration);

        return services;
    }
    private static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Default Connection is not found");

        services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        services.AddIdentity<ApplicationUser,IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();

        return services;
    }
    public static IServiceCollection AddCloudinaryImageHosting(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudinarySettings = configuration.GetSection("Cloudinary");

        var account = new Account(
            cloudinarySettings["CloudName"],
            cloudinarySettings["ApiKey"],
            cloudinarySettings["ApiSecret"]
        );
        services.AddSingleton(new Cloudinary(account));

        services.AddScoped<ICloudinaryService, CloudinaryService>();
        return services;
    }

    public static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("|dpk+&]Y4xaqVCJd3fC+0Q)scm?cqcoMVWk^XXp7@Yn\r\n\r\n")),
                ValidIssuer = "PrimeMarket",
                ValidAudience = "PrimeMarket users"
            };
        });

        return services;
    }
}

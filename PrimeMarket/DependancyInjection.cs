using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using PrimeMarket.Authentication;
using PrimeMarket.Services;
using PrimeMarket.Services.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using MapsterMapper;

namespace PrimeMarket;

public static class DependancyInjection
{
    public static IServiceCollection AddDependancies(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddMapsterConfiguration()
            .AddServiceRegistration()
            .AddDbContextConfiguration(configuration)
            .AddCloudinaryImageHosting(configuration)
            .AddCORSService(configuration)
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

        services.AddHttpContextAccessor();
        return services;
    }
    private static IServiceCollection AddCloudinaryImageHosting(this IServiceCollection services, IConfiguration configuration)
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

        services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        var settings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.Key!)),
                ValidIssuer = settings?.Issuer,
                ValidAudience = settings?.Audience
            };
        });

        return services;
    }

    private static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
    {
        var MappingConfig = TypeAdapterConfig.GlobalSettings;
        MappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(MappingConfig));
        return services;
    }
    private static IServiceCollection AddServiceRegistration(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
    private static IServiceCollection AddCORSService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                )
        );

        return services;
    }
}

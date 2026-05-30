using CloudinaryDotNet;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using PrimeMarket.Authentication;
using PrimeMarket.Services;
using PrimeMarket.Services.Authentication;
using SurveyBasket.Settings;
using System.Reflection;
using System.Text;

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
            .AddValidationConfig()
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
            options.Password.RequiredLength = 1;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
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
        services.AddScoped<IEmailSender, EmailService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserservice, Userservice>();

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

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var response = new { Errors = new[] { "Auth.Unauthorized", "You are not authorized, please login first." } };
                    await context.Response.WriteAsJsonAsync(response);
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var response = new { Errors = new[] { "Auth.Forbidden", "You do not have permission to access this resource." } };
                    await context.Response.WriteAsJsonAsync(response);
                }
            };
        });


        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        services.AddHttpContextAccessor();

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
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IWishListService, WishListService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPromoCodeService, PromoCodeService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IReviewService, ReviewService>();

        services.AddSignalR();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
    private static IServiceCollection AddCORSService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
            options.AddPolicy("AllowAngular", builder =>  // ← named instead of default
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                    .AllowCredentials()
            )
        );

        return services;
    }
    private static IServiceCollection AddValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}

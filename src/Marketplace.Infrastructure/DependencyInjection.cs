using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Marketplace.Application.Common.Interfaces;
using Marketplace.Domain.Interfaces;
using Marketplace.Infrastructure.Identity;
using Marketplace.Infrastructure.Payments;
using Marketplace.Infrastructure.Persistence;

namespace Marketplace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

        services.AddHttpContextAccessor();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Payments:Provider في appsettings بتحدد الـGateway الفعلي —
        // "Mock" للتجربة المحلية بدون أي حساب Stripe، "Stripe" للتكامل الحقيقي بمفاتيح Test Mode
        var paymentsProvider = configuration["Payments:Provider"] ?? "Mock";
        if (paymentsProvider.Equals("Stripe", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IPaymentGateway, StripePaymentGateway>();
        }
        else
        {
            services.AddScoped<IPaymentGateway, MockPaymentGateway>();
        }

        return services;
    }
}

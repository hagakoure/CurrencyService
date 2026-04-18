using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinanceService.Domain.Interfaces;
using FinanceService.Infrastructure.Persistence;
using FinanceService.Infrastructure.Persistence.Repositories;
using FinanceService.Infrastructure.Services;

namespace FinanceService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.EnableRetryOnFailure()));

        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddTransient<IUserContext, UserContext>();
        services.AddHttpContextAccessor();

        return services;
    }
}
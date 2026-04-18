using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Repositories;
using UserService.Infrastructure.Services;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.EnableRetryOnFailure()));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddTransient<IPasswordHasher, AspNetPasswordHasher>();
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
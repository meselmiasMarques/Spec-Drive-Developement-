using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeMoney.Application.Abstractions;
using WeMoney.Infrastructure.Persistence;
using WeMoney.Infrastructure.Persistence.Repositories;
using WeMoney.Infrastructure.Security;

namespace WeMoney.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WeMoneyDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Section))
            .Validate(o => o.Key.Length >= 32, "Jwt:Key deve ser configurada (mínimo 32 caracteres).")
            .ValidateOnStart();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}

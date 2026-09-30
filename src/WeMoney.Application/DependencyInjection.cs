using Microsoft.Extensions.DependencyInjection;
using WeMoney.Application.Auth;
using WeMoney.Application.Categories;
using WeMoney.Application.Transactions;

namespace WeMoney.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AuthService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<TransactionService>();
        return services;
    }
}

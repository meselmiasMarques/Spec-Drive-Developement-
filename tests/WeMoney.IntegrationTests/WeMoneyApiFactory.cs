using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeMoney.Infrastructure.Persistence;

namespace WeMoney.IntegrationTests;

public class WeMoneyApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Jwt:Key", "integration-tests-key-with-at-least-32-chars");
        builder.UseSetting("ConnectionStrings:Default", "Host=unused");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<WeMoneyDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<WeMoneyDbContext>>();
            services.AddDbContext<WeMoneyDbContext>(o => o.UseInMemoryDatabase(_databaseName));
        });
    }
}

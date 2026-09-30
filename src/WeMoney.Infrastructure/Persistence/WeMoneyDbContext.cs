using Microsoft.EntityFrameworkCore;
using WeMoney.Domain.Entities;

namespace WeMoney.Infrastructure.Persistence;

public class WeMoneyDbContext(DbContextOptions<WeMoneyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeMoneyDbContext).Assembly);
    }
}

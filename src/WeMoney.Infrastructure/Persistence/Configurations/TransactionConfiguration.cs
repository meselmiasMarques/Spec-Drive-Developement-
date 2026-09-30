using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeMoney.Domain.Entities;

namespace WeMoney.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Description).HasMaxLength(Transaction.DescriptionMaxLength).IsRequired();
        builder.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Property(t => t.TransactionDate).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        // RN-002 (Categorias): categoria com transações não pode ser excluída.
        builder.HasOne<Category>().WithMany().HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.UserId, t.TransactionDate });
        builder.HasIndex(t => t.CategoryId);
    }
}

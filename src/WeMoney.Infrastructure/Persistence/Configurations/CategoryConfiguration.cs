using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeMoney.Domain.Entities;

namespace WeMoney.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(Category.NameMaxLength).IsRequired();
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(10).IsRequired();

        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(c => c.UserId);
    }
}

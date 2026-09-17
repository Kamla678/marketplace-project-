using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Domain.Entities;

namespace Marketplace.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(250).IsRequired();
        builder.Property(p => p.Slug).HasMaxLength(280).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.Description).HasColumnType("nvarchar(max)");
        builder.Property(p => p.Price).HasColumnType("decimal(10,2)");
        builder.Property(p => p.DiscountPrice).HasColumnType("decimal(10,2)");
        builder.Property(p => p.AvgRating).HasColumnType("decimal(3,2)");

        builder.Property(p => p.SKU).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => p.SKU).IsUnique();

        builder.Property(p => p.ApprovalStatus).HasConversion<int>();

        builder.HasOne(p => p.Seller)
            .WithMany()
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany()
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // أهم Indexes للأداء — راجعي phase5-search-performance.md
        builder.HasIndex(p => new { p.CategoryId, p.Price });
        builder.HasIndex(p => p.SellerId);
        builder.HasIndex(p => new { p.ApprovalStatus, p.IsActive });
    }
}

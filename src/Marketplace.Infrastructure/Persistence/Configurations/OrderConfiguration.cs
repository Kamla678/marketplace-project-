using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Domain.Entities;

namespace Marketplace.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.Status).HasConversion<int>();
        builder.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
        builder.Property(o => o.DiscountAmount).HasColumnType("decimal(10,2)");
        builder.Property(o => o.ShippingFee).HasColumnType("decimal(10,2)");
        builder.Property(o => o.TotalAmount).HasColumnType("decimal(10,2)");

        builder.Property(o => o.ShippingFullName).HasMaxLength(150).IsRequired();
        builder.Property(o => o.ShippingAddressLine).HasMaxLength(300).IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // أهم index هنا — راجعي phase5-search-performance.md
        builder.HasIndex(o => new { o.UserId, o.CreatedAt });
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");
        builder.Property(i => i.Subtotal).HasColumnType("decimal(10,2)");
        builder.Property(i => i.ItemStatus).HasConversion<int>();

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.SellerId); // للـSeller dashboard queries
    }
}

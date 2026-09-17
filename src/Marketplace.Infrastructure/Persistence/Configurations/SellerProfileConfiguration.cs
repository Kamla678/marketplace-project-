using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Domain.Entities;

namespace Marketplace.Infrastructure.Persistence.Configurations;

public class SellerProfileConfiguration : IEntityTypeConfiguration<SellerProfile>
{
    public void Configure(EntityTypeBuilder<SellerProfile> builder)
    {
        builder.ToTable("SellerProfiles");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StoreName).HasMaxLength(150).IsRequired();
        builder.Property(s => s.StoreDescription).HasMaxLength(1000);
        builder.Property(s => s.ApprovalStatus).HasConversion<int>();
        builder.Property(s => s.CommissionRate).HasColumnType("decimal(5,2)");

        builder.HasIndex(s => s.UserId).IsUnique();
    }
}

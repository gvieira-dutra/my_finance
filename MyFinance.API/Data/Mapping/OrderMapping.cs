using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Core.Models;

namespace MyFinance.API.Data.Mapping;

public class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .IsRequired(true)
            .HasColumnType("CHAR")
            .HasMaxLength(8);

        builder.Property(x => x.ExternalReference)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(60);

        builder.Property(x => x.Gateway)
            .IsRequired(true)
            .HasColumnType("SMALLINT");

        builder.Property(x => x.CreatedAt)
            .IsRequired(true)
            .HasColumnType("DATETIME2");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(true)
            .HasColumnType("DATETIME2");

        builder.Property(x => x.Status)
            .IsRequired(true)
            .HasColumnType("SMALLINT");

        builder.Property(x => x.UserId)
            .IsRequired(false)
            .HasColumnType("VARCHAR")
            .HasMaxLength(80);

        builder.HasOne(x => x.Product).WithMany();
        builder.HasOne(x => x.Voucher).WithMany();
    }
}

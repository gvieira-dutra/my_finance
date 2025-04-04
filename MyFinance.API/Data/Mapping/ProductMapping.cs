﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Core.Models;

namespace MyFinance.API.Data.Mapping;

public class ProductMapping : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired(true)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255);

        builder.Property(x => x.Price)
            .IsRequired(true)
            .HasColumnType("MONEY");

        builder.Property(x => x.IsActive)
            .IsRequired(true)
            .HasColumnType("BIT");

        builder.Property(x => x.Slug)
            .IsRequired(true)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Summary)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(225);
    }
}

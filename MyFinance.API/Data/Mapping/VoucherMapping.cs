﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyFinance.Core.Models;

namespace MyFinance.API.Data.Mapping;

public class VoucherMapping : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Voucher");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Number)
            .IsRequired(true)
            .HasColumnType("CHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Title)
            .IsRequired(true)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Amount)
            .IsRequired(true)
            .HasColumnType("MONEY");

        builder.Property(x => x.IsActive)
            .IsRequired(true)
            .HasColumnType("BIT");
    }
}

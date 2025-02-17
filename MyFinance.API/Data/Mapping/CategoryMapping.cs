// Ignore Spelling: API

using Microsoft.EntityFrameworkCore;
using MyFinance.Core.Models;

namespace MyFinance.API.Data.Mapping;

//IEntityTypeConfiguration, EntityTypeBuilder
//are provided by package Microsoft.EntityFrameworkCore.SqlServer 

public class CategoryMapping : IEntityTypeConfiguration<Category>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired(true)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasColumnType("NVARCHAR")
            .HasMaxLength(255); ;

        builder.Property(x => x.UserId)
            .IsRequired(true)
            .HasColumnType("VARCHAR")
            .HasMaxLength(160);
    }
}

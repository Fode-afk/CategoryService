using CategoryService.Domain.Models;
using CategoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CategoryService.Infrastructure.Data.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", Schemas.CategoryWrite);

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.ParentCategoryId);
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.IsActive);

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => CategoryName.Create(value).Value)
            .HasMaxLength(CategoryName.MaxLength);

        builder.Property(x => x.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value).Value)
            .HasMaxLength(Slug.MaxLength);

        builder.OwnsOne(p => p.SeoMetadata, seoBuilder =>
        {
            seoBuilder.Property(m => m.Title)
                .HasMaxLength(SeoTitle.MaxLength)
                .HasColumnName("SeoTitle")
                .HasConversion(
                    title => title.Value,
                    value => SeoTitle.Create(value).Value);

            seoBuilder.Property(m => m.Description)
                .HasMaxLength(SeoDescription.MaxLength)
                .HasColumnName("SeoDescription")
                .HasConversion(
                    description => description.Value,
                    value => SeoDescription.Create(value).Value);

            seoBuilder.Property(m => m.Keywords)
                .HasMaxLength(SeoKeywords.MaxLength)
                .HasColumnName("SeoKeywords")
                .HasConversion(
                    keywords => keywords.Value,
                    value => SeoKeywords.Create(value).Value);
        });

        builder.Property(x => x.Image)
            .HasConversion(
                imageUrl => imageUrl == null ? null : imageUrl.Value,
                value => value == null ? null : ImageUrl.Create(value).Value);

        builder.Property(x => x.RowVersion)
          .IsRowVersion()
          .IsConcurrencyToken();

        builder.Property(x => x.Version)
            .IsRequired();
    }
}

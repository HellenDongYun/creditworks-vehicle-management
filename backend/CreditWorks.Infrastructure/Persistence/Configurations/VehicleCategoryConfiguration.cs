using CreditWorks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWorks.Infrastructure.Persistence.Configurations;

public sealed class VehicleCategoryConfiguration
    : IEntityTypeConfiguration<VehicleCategory>
{
    public void Configure(
        EntityTypeBuilder<VehicleCategory> builder)
    {
        builder.ToTable("VehicleCategories", table =>
        {
            table.HasCheckConstraint(
                "CK_Categories_MinWeight",
                "[MinWeight] >= 0");

            table.HasCheckConstraint(
                "CK_Categories_Range",
                "[MaxWeight] IS NULL OR [MaxWeight] > [MinWeight]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Icon)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(x => x.Range, range =>
        {
            range.Property(x => x.MinWeight)
                .HasColumnName("MinWeight")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            range.Property(x => x.MaxWeight)
                .HasColumnName("MaxWeight")
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);
        });

        builder.Navigation(x => x.Range)
            .IsRequired();
    }
}
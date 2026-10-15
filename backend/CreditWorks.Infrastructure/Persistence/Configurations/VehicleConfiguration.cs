using CreditWorks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditWorks.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration
    : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles", table =>
        {
            table.HasCheckConstraint(
                "CK_Vehicles_Weight_Positive",
                "[Weight] > 0");

            table.HasCheckConstraint(
                "CK_Vehicles_Year",
                "[YearOfManufacture] >= 1886");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Weight)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne(x => x.Manufacturer)
            .WithMany()
            .HasForeignKey(x => x.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ManufacturerId);
    }
}
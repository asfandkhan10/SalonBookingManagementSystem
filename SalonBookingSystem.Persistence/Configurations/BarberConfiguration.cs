using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Persistence.Configurations;

public class BarberConfiguration : IEntityTypeConfiguration<Barber>
{
    public void Configure(EntityTypeBuilder<Barber> builder)
    {
        builder.ToTable("Barbers");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.IsActive)
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .IsRequired();

        builder.Property(b => b.UpdatedBy);

        builder.HasIndex(b => b.IsActive);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}

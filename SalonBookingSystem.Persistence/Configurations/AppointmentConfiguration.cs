using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CustomerId)
            .IsRequired();

        builder.Property(a => a.BarberId)
            .IsRequired();

        builder.Property(a => a.AppointmentDate)
            .IsRequired();

        builder.Property(a => a.StartTime)
            .IsRequired();

        builder.Property(a => a.EndTime)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.TotalDurationMinutes)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .IsRequired();

        builder.Property(a => a.UpdatedBy);

        builder.HasIndex(a => new { a.BarberId, a.AppointmentDate });

        builder.HasIndex(a => a.CustomerId);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

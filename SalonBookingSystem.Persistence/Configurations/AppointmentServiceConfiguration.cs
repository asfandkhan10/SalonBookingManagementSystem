using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Persistence.Configurations;

public class AppointmentServiceConfiguration : IEntityTypeConfiguration<AppointmentService>
{
    public void Configure(EntityTypeBuilder<AppointmentService> builder)
    {
        builder.ToTable("AppointmentServices");

        builder.HasKey(as_ => as_.Id);

        builder.Property(as_ => as_.AppointmentId)
            .IsRequired();

        builder.Property(as_ => as_.ServiceId)
            .IsRequired();

        builder.Property(as_ => as_.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(as_ => as_.DurationMinutes)
            .IsRequired();

        builder.Property(as_ => as_.CreatedBy)
            .IsRequired();

        builder.Property(as_ => as_.UpdatedBy);

        builder.HasIndex(as_ => as_.AppointmentId);

        builder.HasQueryFilter(as_ => !as_.IsDeleted);
    }
}

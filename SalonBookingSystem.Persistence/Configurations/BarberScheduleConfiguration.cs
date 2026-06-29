using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalonBookingSystem.Domain.Entities;

namespace SalonBookingSystem.Persistence.Configurations;

public class BarberScheduleConfiguration : IEntityTypeConfiguration<BarberSchedule>
{
    public void Configure(EntityTypeBuilder<BarberSchedule> builder)
    {
        builder.ToTable("BarberSchedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.BarberId)
            .IsRequired();

        builder.Property(s => s.DayOfWeek)
            .IsRequired();

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .IsRequired();

        builder.Property(s => s.UpdatedBy);

        builder.HasIndex(s => new { s.BarberId, s.DayOfWeek });

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

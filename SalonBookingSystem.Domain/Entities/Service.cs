using SalonBookingSystem.Domain.Common;

namespace SalonBookingSystem.Domain.Entities;

public class Service : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }
}

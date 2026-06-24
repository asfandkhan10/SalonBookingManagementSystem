namespace SalonBookingSystem.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; } = 0;

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; } = 0;

    public bool IsDeleted { get; set; }
}

namespace SalonBookingSystem.Application.DTOs.Service;

public class UpdateServiceRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }
}

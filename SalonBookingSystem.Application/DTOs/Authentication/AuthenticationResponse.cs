namespace SalonBookingSystem.Application.DTOs.Authentication;

public class AuthenticationResponse
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Token { get; set; }

    public int? CustomerId { get; set; }
}

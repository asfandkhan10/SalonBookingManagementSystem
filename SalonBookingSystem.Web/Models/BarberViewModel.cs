namespace SalonBookingSystem.Web.Models;

public class BarberViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}

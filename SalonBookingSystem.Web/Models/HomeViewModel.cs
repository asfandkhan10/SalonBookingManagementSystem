namespace SalonBookingSystem.Web.Models;

public class HomeViewModel
{
    public List<ServiceViewModel> Services { get; set; } = new();
    public List<BarberViewModel> Barbers { get; set; } = new();
}

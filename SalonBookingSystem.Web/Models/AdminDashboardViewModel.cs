namespace SalonBookingSystem.Web.Models;

public class AdminDashboardViewModel
{
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UpcomingBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CompletedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public int TodayBookings { get; set; }
    public int TotalBarbers { get; set; }
    public int ActiveBarbers { get; set; }
    public int TotalServices { get; set; }
    public int TotalCustomers { get; set; }
    public List<AppointmentViewModel> RecentAppointments { get; set; } = new();
}

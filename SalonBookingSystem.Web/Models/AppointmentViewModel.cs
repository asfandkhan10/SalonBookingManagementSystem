namespace SalonBookingSystem.Web.Models;

public class AppointmentViewModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int BarberId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TotalDurationMinutes { get; set; }
    public string? CustomerName { get; set; }
    public string? BarberName { get; set; }

    /// <summary>Badge CSS class based on status for the dark theme.</summary>
    public string StatusBadgeClass => Status switch
    {
        "Completed" => "bg-green-500/20 text-green-400 border border-green-500/30",
        "Cancelled"  => "bg-red-500/20 text-red-400 border border-red-500/30",
        "Confirmed"  => "bg-yellow-500/20 text-yellow-400 border border-yellow-500/30",
        _            => "bg-gray-500/20 text-gray-400 border border-gray-500/30"
    };

    public bool CanCancel => Status is "Pending" or "Confirmed";
}

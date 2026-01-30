namespace PCM.Application.DTOs.Bookings;

public class TimeSlotDto
{
    public string StartTime { get; set; } = string.Empty; // "08:00"
    public string EndTime { get; set; } = string.Empty;   // "09:00"
    public bool IsAvailable { get; set; }
    public string? BookedBy { get; set; }
    public int? BookingId { get; set; }
}

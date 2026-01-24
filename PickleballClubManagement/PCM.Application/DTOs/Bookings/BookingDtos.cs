using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Bookings;

public class BookingDto
{
    public int Id { get; set; }
    public int CourtId { get; set; }
    public string CourtName { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class BookingCreateDto
{
    public int CourtId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}

public class RecurringBookingDto
{
    public int CourtId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Notes { get; set; }
}

public class AvailableSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
}

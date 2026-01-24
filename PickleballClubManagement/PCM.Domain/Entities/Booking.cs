using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PCM.Domain.Enums;

namespace PCM.Domain.Entities;

[Table("189_Bookings")]
public class Booking
{
    [Key]
    public int Id { get; set; }

    public int CourtId { get; set; }
    public int MemberId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("CourtId")]
    public virtual Court? Court { get; set; }

    [ForeignKey("MemberId")]
    public virtual Member? Member { get; set; }
}
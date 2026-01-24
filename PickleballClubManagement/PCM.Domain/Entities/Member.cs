using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("189_Members")]
public class Member
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    [Required]
    public DateTime JoinDate { get; set; }

    public double RankELO { get; set; } = 1200.0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal WalletBalance { get; set; } = 0;

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int TotalMatches { get; set; } = 0;

    public int WinMatches { get; set; } = 0;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    // Navigation properties
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

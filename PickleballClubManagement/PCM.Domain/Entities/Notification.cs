using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PCM.Domain.Enums;

namespace PCM.Domain.Entities;

[Table("189_Notifications")]
public class Notification
{
    [Key]
    public int Id { get; set; }

    public int? MemberId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    // Navigation properties
    [ForeignKey("MemberId")]
    public virtual Member? Member { get; set; }
}

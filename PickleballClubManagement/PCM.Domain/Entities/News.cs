using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("189_News")]
public class News
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsPinned { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [MaxLength(200)]
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}

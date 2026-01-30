using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("MatchSets")]
public class MatchSet
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }

    [Required]
    public int SetNumber { get; set; } // 1, 2, 3...

    [Required]
    public int Team1Score { get; set; } // Score for Team/Player 1 in this set

    [Required]
    public int Team2Score { get; set; } // Score for Team/Player 2 in this set

    public bool IsFinalSet { get; set; } // Indicates if this was the deciding set

    [Required]
    public int WinningSide { get; set; } // 1 or 2

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey("MatchId")]
    public virtual Match? Match { get; set; }
}

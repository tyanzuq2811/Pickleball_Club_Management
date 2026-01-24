using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("189_MatchScores")]
public class MatchScore
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }

    [Required]
    public int SetNumber { get; set; }

    [Required]
    public int Team1Score { get; set; }

    [Required]
    public int Team2Score { get; set; }

    public bool IsFinalSet { get; set; }

    // Navigation properties
    [ForeignKey("MatchId")]
    public virtual Match Match { get; set; } = null!;
}

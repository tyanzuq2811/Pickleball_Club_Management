using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("189_TournamentMatches")]
public class TournamentMatch
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TournamentId { get; set; }

    [Required]
    public int Round { get; set; }

    [Required]
    public int MatchId { get; set; }

    public int? NextMatchId { get; set; }

    public int? ParentMatchId { get; set; }

    [MaxLength(100)]
    public string? BracketGroup { get; set; }

    // Navigation properties
    [ForeignKey("TournamentId")]
    public virtual Tournament Tournament { get; set; } = null!;

    [ForeignKey("MatchId")]
    public virtual Match Match { get; set; } = null!;
}

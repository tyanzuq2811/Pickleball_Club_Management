using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PCM.Domain.Enums;

namespace PCM.Domain.Entities;

[Table("189_Matches")]
public class Match
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public bool IsRanked { get; set; }

    public int? TournamentId { get; set; }

    [Required]
    public MatchFormat MatchFormat { get; set; }

    public int? Team1_Player1Id { get; set; }

    public int? Team1_Player2Id { get; set; }

    public int? Team2_Player1Id { get; set; }

    public int? Team2_Player2Id { get; set; }

    [Required]
    public WinningSide WinningSide { get; set; }

    public double EloChange { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("TournamentId")]
    public virtual Tournament? Tournament { get; set; }

    [ForeignKey("Team1_Player1Id")]
    public virtual Member? Team1_Player1 { get; set; }

    [ForeignKey("Team1_Player2Id")]
    public virtual Member? Team1_Player2 { get; set; }

    [ForeignKey("Team2_Player1Id")]
    public virtual Member? Team2_Player1 { get; set; }

    [ForeignKey("Team2_Player2Id")]
    public virtual Member? Team2_Player2 { get; set; }

    public virtual ICollection<MatchScore> MatchScores { get; set; } = new List<MatchScore>();
}

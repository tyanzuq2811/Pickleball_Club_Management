using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PCM.Domain.Enums;

namespace PCM.Domain.Entities;

[Table("189_Tournaments")]
public class Tournament
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public TournamentType Type { get; set; }

    [Required]
    public GameMode GameMode { get; set; }

    [Required]
    public TournamentStatus Status { get; set; }

    public int? Config_TargetWins { get; set; }

    public int CurrentScore_TeamA { get; set; } = 0;

    public int CurrentScore_TeamB { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal EntryFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrizePool { get; set; } = 0;

    [Required]
    public int CreatedBy { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedDate { get; set; }

    [MaxLength(4000)]
    public string? ConfigData { get; set; }

    // Navigation properties
    [ForeignKey("CreatedBy")]
    public virtual Member Creator { get; set; } = null!;

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual ICollection<TournamentMatch> TournamentMatches { get; set; } = new List<TournamentMatch>();
}

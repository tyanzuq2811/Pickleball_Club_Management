using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PCM.Domain.Enums;

namespace PCM.Domain.Entities;

[Table("189_Participants")]
public class Participant
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TournamentId { get; set; }

    [Required]
    public int MemberId { get; set; }

    [Required]
    public TeamSide Team { get; set; }

    public bool EntryFeePaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal EntryFeeAmount { get; set; }

    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public ParticipantStatus Status { get; set; }

    public int? SeedNo { get; set; }

    // Navigation properties
    [ForeignKey("TournamentId")]
    public virtual Tournament Tournament { get; set; } = null!;

    [ForeignKey("MemberId")]
    public virtual Member Member { get; set; } = null!;
}

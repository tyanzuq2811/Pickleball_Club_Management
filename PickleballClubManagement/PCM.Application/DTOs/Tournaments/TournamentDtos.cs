using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Tournaments;

public class TournamentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TournamentType Type { get; set; }
    public GameMode GameMode { get; set; }
    public TournamentStatus Status { get; set; }
    public int? ConfigTargetWins { get; set; }
    public int CurrentScoreTeamA { get; set; }
    public int CurrentScoreTeamB { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public int CreatedBy { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public int ParticipantCount { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
}

public class TournamentCreateDto
{
    public string Title { get; set; } = string.Empty;
    public TournamentType Type { get; set; }
    public GameMode GameMode { get; set; }
    public int? ConfigTargetWins { get; set; }
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public DateTime? StartDate { get; set; }
}

public class TournamentUpdateDto
{
    public string? Title { get; set; }
    public TournamentStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ParticipantDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public TeamSide Team { get; set; }
    public bool EntryFeePaid { get; set; }
    public decimal EntryFeeAmount { get; set; }
    public ParticipantStatus Status { get; set; }
    public int? SeedNo { get; set; }
    public DateTime JoinedDate { get; set; }
}

public class JoinTournamentDto
{
    public int TournamentId { get; set; }
}

using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Matches;

public class MatchDto
{
    public int Id { get; set; }
    public string Team1Name { get; set; } = string.Empty;
    public string Team2Name { get; set; } = string.Empty;
    public string? Team1Player1Name { get; set; }
    public string? Team1Player2Name { get; set; }
    public string? Team2Player1Name { get; set; }
    public string? Team2Player2Name { get; set; }
    public int? Team1Player1Id { get; set; }
    public int? Team1Player2Id { get; set; }
    public int? Team2Player1Id { get; set; }
    public int? Team2Player2Id { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public string Status { get; set; } = "Scheduled";
    public DateTime Date { get; set; }
    public int? TournamentId { get; set; }
    public string? TournamentTitle { get; set; }
    public string MatchFormat { get; set; } = "Singles";
    public bool IsRanked { get; set; }
    public double EloChange { get; set; }
    public int WinningSide { get; set; }
}
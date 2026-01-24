using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Tournaments;

public class TournamentBracketDto
{
    public int TournamentId { get; set; }
    public List<BracketRoundDto> Rounds { get; set; } = new();
}

public class BracketRoundDto
{
    public int RoundNumber { get; set; }
    public List<BracketMatchDto> Matches { get; set; } = new();
}

public class BracketMatchDto
{
    public int MatchId { get; set; }
    public string? Team1Player1 { get; set; }
    public string? Team2Player1 { get; set; }
    public int? Team1Score { get; set; } // Simplified for display
    public int? Team2Score { get; set; }
    public WinningSide WinningSide { get; set; }
    public int? NextMatchId { get; set; }
}
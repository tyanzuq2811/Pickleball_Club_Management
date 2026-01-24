using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Matches;

public class MatchDto
{
    public int Id { get; set; }
    public string Team1Name { get; set; } = string.Empty;
    public string Team2Name { get; set; } = string.Empty;
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public string Status { get; set; } = "Scheduled";
    public DateTime Date { get; set; }
}
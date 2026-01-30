namespace PCM.Application.DTOs.Matches;

public class MatchSetDto
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int SetNumber { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public bool IsFinalSet { get; set; }
    public int WinningSide { get; set; } // 1 or 2
}

public class CreateMatchSetDto
{
    public int MatchId { get; set; }
    public int SetNumber { get; set; }
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public bool IsFinalSet { get; set; }
}

public class MatchWithSetsDto
{
    public int Id { get; set; }
    public string Player1Name { get; set; } = string.Empty;
    public string? Player2Name { get; set; }
    public string Player3Name { get; set; } = string.Empty;
    public string? Player4Name { get; set; }
    public int Score1 { get; set; }
    public int Score2 { get; set; }
    public int WinningSide { get; set; }
    public string Format { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<MatchSetDto> Sets { get; set; } = new();
}

public class UpdateMatchScoreDto
{
    public int? Team1Score { get; set; }
    public int? Team2Score { get; set; }
    public int? WinningSide { get; set; } // 0=None, 1=Team1, 2=Team2
}

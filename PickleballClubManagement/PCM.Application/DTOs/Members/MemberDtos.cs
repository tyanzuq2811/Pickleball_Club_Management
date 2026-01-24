namespace PCM.Application.DTOs.Members;

public class MemberDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }
    public double RankELO { get; set; }
    public decimal WalletBalance { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public int TotalMatches { get; set; }
    public int WinMatches { get; set; }
    public double WinRate => TotalMatches > 0 ? (double)WinMatches / TotalMatches * 100 : 0;
}

public class MemberCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public double RankELO { get; set; } = 1200;
}

public class MemberUpdateDto
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
}

public class MemberRankingDto
{
    public int Rank { get; set; }
    public int MemberId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public double RankELO { get; set; }
    public int TotalMatches { get; set; }
    public int WinMatches { get; set; }
    public double WinRate { get; set; }
    public string? AvatarUrl { get; set; }
}

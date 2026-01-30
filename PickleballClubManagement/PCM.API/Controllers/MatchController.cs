using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Matches; // Cần tạo DTO này
using PCM.Application.Interfaces;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchController : ControllerBase
{
    // private readonly IMatchService _matchService; // Cần tạo Service này
    private readonly ITournamentService _tournamentService; // Tạm dùng TournamentService nếu logic match nằm trong đó

    public MatchController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MatchDto>>>> GetAllMatches()
    {
        var result = await _tournamentService.GetAllMatchesAsync();
        return Ok(result);
    }

    [HttpPut("{id}/score")]
    [Authorize(Roles = "Referee,Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateScore(int id, [FromBody] UpdateScoreDto request)
    {
        var result = await _tournamentService.UpdateMatchResultAsync(id, request.Team1Score, request.Team2Score, request.SetNumber, request.IsFinal);
        return Ok(result);
    }
}

public class UpdateScoreDto
{
    public int Team1Score { get; set; }
    public int Team2Score { get; set; }
    public int SetNumber { get; set; } = 1;
    public bool IsFinal { get; set; }
}
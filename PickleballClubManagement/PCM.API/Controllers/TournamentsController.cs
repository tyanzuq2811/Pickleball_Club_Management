using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PCM.API.Hubs;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Tournaments;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly IMemberService _memberService;
    private readonly IHubContext<ScoreboardHub> _hubContext;

    public TournamentsController(
        ITournamentService tournamentService, 
        IMemberService memberService,
        IHubContext<ScoreboardHub> hubContext)
    {
        _tournamentService = tournamentService;
        _memberService = memberService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Lấy danh sách giải đấu
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<TournamentDto>>>> GetAllTournaments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _tournamentService.GetAllAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy các trận đấu giải đấu đang diễn ra
    /// </summary>
    [HttpGet("live-matches")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<object>>>> GetLiveMatches()
    {
        var result = await _tournamentService.GetLiveMatchesAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// L?y th�ng tin gi?i ??u theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> GetTournamentById(int id)
    {
        var result = await _tournamentService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// T?o gi?i ??u m?i
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> CreateTournament([FromBody] TournamentCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member?.Data == null)
            return Unauthorized(ApiResponse<TournamentDto>.ErrorResponse("Kh�ng t�m th?y th�ng tin th�nh vi�n"));

        var result = await _tournamentService.CreateAsync(dto, member.Data.Id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// C?p nh?t gi?i ??u
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> UpdateTournament(int id, [FromBody] TournamentUpdateDto dto)
    {
        var result = await _tournamentService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tham gia gi?i ??u
    /// </summary>
    [HttpPost("{id}/join")]
    public async Task<ActionResult<ApiResponse<bool>>> JoinTournament(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
            
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member?.Data == null)
            return Unauthorized(ApiResponse<bool>.ErrorResponse("Kh�ng t�m th?y th�ng tin th�nh vi�n"));

        var result = await _tournamentService.JoinTournamentAsync(id, member.Data.Id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// T? ??ng chia ??i
    /// </summary>
    [HttpPost("{id}/auto-divide-teams")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<bool>>> AutoDivideTeams(int id)
    {
        var result = await _tournamentService.AutoDivideTeamsAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tạo bracket cho giải đấu
    /// </summary>
    [HttpPost("{id}/generate-bracket")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<bool>>> GenerateBracket(int id)
    {
        var result = await _tournamentService.GenerateBracketAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// L?y th�ng tin bracket
    /// </summary>
    [HttpGet("{id}/bracket")]
    public async Task<ActionResult<ApiResponse<object>>> GetBracket(int id)
    {
        var result = await _tournamentService.GetBracketAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy danh sách người tham gia giải đấu
    /// </summary>
    [HttpGet("{id}/participants")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<List<ParticipantDto>>>> GetParticipants(int id)
    {
        var result = await _tournamentService.GetParticipantsAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Cập nhật tỷ số giải đấu (Team Battle)
    /// </summary>
    [HttpPost("{id}/score")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateTournamentScore(int id, [FromQuery] int teamA, [FromQuery] int teamB)
    {
        var result = await _tournamentService.UpdateScoreAsync(id, teamA, teamB);
        if (result.Success)
        {
            await _hubContext.Clients.All.SendAsync("TournamentScoreUpdated", new { TournamentId = id, TeamA = teamA, TeamB = teamB });
        }
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("matches/{matchId}/score")]
    [Authorize(Roles = "Admin,Referee")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateMatchScore(int matchId, [FromQuery] int t1, [FromQuery] int t2, [FromQuery] int set = 1, [FromQuery] bool final = false)
    {
        var result = await _tournamentService.UpdateMatchResultAsync(matchId, t1, t2, set, final);
        if (result.Success)
        {
            await _hubContext.Clients.Group($"Match_{matchId}").SendAsync("MatchScoreUpdated", new { MatchId = matchId, T1 = t1, T2 = t2, Set = set, Final = final });
        }
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
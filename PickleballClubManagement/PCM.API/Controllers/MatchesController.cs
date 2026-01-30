using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Matches;
using PCM.Domain.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Enums;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public MatchesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Lấy danh sách tất cả trận đấu
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<MatchDto>>>> GetAllMatches([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var matches = await _unitOfWork.Matches.GetAllAsync();
            var total = matches.Count();
            
            // Lấy danh sách member IDs và tournament IDs để query
            var memberIds = matches
                .SelectMany(m => new[] { m.Team1_Player1Id, m.Team1_Player2Id, m.Team2_Player1Id, m.Team2_Player2Id })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();
            
            var tournamentIds = matches
                .Where(m => m.TournamentId.HasValue)
                .Select(m => m.TournamentId!.Value)
                .Distinct()
                .ToList();
            
            // Lấy thông tin members và tournaments
            var members = await _unitOfWork.Members.FindAsync(m => memberIds.Contains(m.Id));
            var memberDict = members.ToDictionary(m => m.Id, m => m.FullName);
            
            var tournaments = await _unitOfWork.Tournaments.FindAsync(t => tournamentIds.Contains(t.Id));
            var tournamentDict = tournaments.ToDictionary(t => t.Id, t => t.Title);
            
            // Lấy match scores
            var matchIds = matches.Select(m => m.Id).ToList();
            var matchScores = await _unitOfWork.MatchScores.FindAsync(s => matchIds.Contains(s.MatchId));
            var scoresByMatch = matchScores.GroupBy(s => s.MatchId).ToDictionary(
                g => g.Key, 
                g => new { 
                    Team1 = g.Sum(s => s.Team1Score > s.Team2Score ? 1 : 0),
                    Team2 = g.Sum(s => s.Team2Score > s.Team1Score ? 1 : 0)
                });
            
            var items = matches
                .OrderByDescending(m => m.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => {
                    var t1p1 = m.Team1_Player1Id.HasValue && memberDict.TryGetValue(m.Team1_Player1Id.Value, out var n1) ? n1 : null;
                    var t1p2 = m.Team1_Player2Id.HasValue && memberDict.TryGetValue(m.Team1_Player2Id.Value, out var n2) ? n2 : null;
                    var t2p1 = m.Team2_Player1Id.HasValue && memberDict.TryGetValue(m.Team2_Player1Id.Value, out var n3) ? n3 : null;
                    var t2p2 = m.Team2_Player2Id.HasValue && memberDict.TryGetValue(m.Team2_Player2Id.Value, out var n4) ? n4 : null;
                    
                    var team1Name = t1p2 != null ? $"{t1p1} & {t1p2}" : t1p1 ?? "TBD";
                    var team2Name = t2p2 != null ? $"{t2p1} & {t2p2}" : t2p1 ?? "TBD";
                    
                    var scores = scoresByMatch.TryGetValue(m.Id, out var s) ? s : new { Team1 = 0, Team2 = 0 };
                    
                    return new MatchDto
                    {
                        Id = m.Id,
                        Date = m.Date,
                        Status = m.WinningSide == WinningSide.None ? "Scheduled" : "Completed",
                        Team1Name = team1Name,
                        Team2Name = team2Name,
                        Team1Player1Name = t1p1,
                        Team1Player2Name = t1p2,
                        Team2Player1Name = t2p1,
                        Team2Player2Name = t2p2,
                        Team1Player1Id = m.Team1_Player1Id,
                        Team1Player2Id = m.Team1_Player2Id,
                        Team2Player1Id = m.Team2_Player1Id,
                        Team2Player2Id = m.Team2_Player2Id,
                        Team1Score = scores.Team1,
                        Team2Score = scores.Team2,
                        TournamentId = m.TournamentId,
                        TournamentTitle = m.TournamentId.HasValue && tournamentDict.TryGetValue(m.TournamentId.Value, out var tt) ? tt : null,
                        MatchFormat = m.MatchFormat.ToString(),
                        IsRanked = m.IsRanked,
                        EloChange = m.EloChange,
                        WinningSide = (int)m.WinningSide
                    };
                })
                .ToList();

            var result = new PagedResult<MatchDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(ApiResponse<PagedResult<MatchDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<MatchDto>>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy thông tin trận đấu theo ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<MatchDto>>> GetMatchById(int id)
    {
        try
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null)
                return NotFound(ApiResponse<MatchDto>.ErrorResponse("Match not found"));

            var dto = new MatchDto
            {
                Id = match.Id,
                Date = match.Date,
                Status = match.WinningSide == WinningSide.None ? "Scheduled" : "Completed",
                Team1Name = $"Team 1",
                Team2Name = $"Team 2",
                Team1Score = 0,
                Team2Score = 0
            };

            return Ok(ApiResponse<MatchDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MatchDto>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Lấy chi tiết trận đấu kèm theo sets
    /// </summary>
    [HttpGet("{id}/sets")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<MatchWithSetsDto>>> GetMatchWithSets(int id)
    {
        try
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null)
                return NotFound(ApiResponse<MatchWithSetsDto>.ErrorResponse("Match not found"));

            // Get sets from repository (you'll need to implement this)
            var sets = new List<MatchSetDto>(); // TODO: Fetch from database

            var dto = new MatchWithSetsDto
            {
                Id = match.Id,
                Player1Name = "Player 1",
                Player2Name = "Player 2",
                Player3Name = "Player 3",
                Player4Name = "Player 4",
                Score1 = 0,
                Score2 = 0,
                WinningSide = (int)match.WinningSide,
                Format = match.MatchFormat.ToString(),
                StartTime = match.Date,
                EndTime = match.Date.AddHours(1),
                Sets = sets
            };

            return Ok(ApiResponse<MatchWithSetsDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MatchWithSetsDto>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Cập nhật tỷ số và kết quả trận đấu
    /// </summary>
    [HttpPut("{id}/score")]
    [Authorize(Roles = "Referee,Admin")]
    public async Task<ActionResult<ApiResponse<MatchDto>>> UpdateMatchScore(int id, [FromBody] UpdateMatchScoreDto dto)
    {
        try
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null)
                return NotFound(ApiResponse<MatchDto>.ErrorResponse("Không tìm thấy trận đấu"));

            // Cập nhật winning side
            if (dto.WinningSide.HasValue)
            {
                match.WinningSide = (WinningSide)dto.WinningSide.Value;
            }

            // Xóa match scores cũ và thêm mới nếu có điểm
            if (dto.Team1Score.HasValue && dto.Team2Score.HasValue)
            {
                var existingScores = await _unitOfWork.MatchScores.FindAsync(s => s.MatchId == id);
                foreach (var score in existingScores)
                {
                    _unitOfWork.MatchScores.Remove(score);
                }

                // Thêm score mới
                var matchScore = new MatchScore
                {
                    MatchId = id,
                    SetNumber = 1,
                    Team1Score = dto.Team1Score.Value,
                    Team2Score = dto.Team2Score.Value,
                    IsFinalSet = true
                };
                await _unitOfWork.MatchScores.AddAsync(matchScore);
            }

            await _unitOfWork.SaveChangesAsync();

            // Cập nhật ELO nếu trận ranked và đã có người thắng
            if (match.IsRanked && match.WinningSide != WinningSide.None)
            {
                await UpdateEloRatings(match);
            }

            return Ok(ApiResponse<MatchDto>.SuccessResponse(null, "Cập nhật tỷ số thành công"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MatchDto>.ErrorResponse($"Lỗi: {ex.Message}"));
        }
    }

    private async Task UpdateEloRatings(Match match)
    {
        const double K = 32; // K-factor cho ELO
        
        var winnerId = match.WinningSide == WinningSide.Team1 ? match.Team1_Player1Id : match.Team2_Player1Id;
        var loserId = match.WinningSide == WinningSide.Team1 ? match.Team2_Player1Id : match.Team1_Player1Id;
        
        if (!winnerId.HasValue || !loserId.HasValue) return;
        
        var winner = await _unitOfWork.Members.GetByIdAsync(winnerId.Value);
        var loser = await _unitOfWork.Members.GetByIdAsync(loserId.Value);
        
        if (winner == null || loser == null) return;

        // Calculate expected scores
        double expectedWinner = 1 / (1 + Math.Pow(10, (loser.RankELO - winner.RankELO) / 400));
        double expectedLoser = 1 / (1 + Math.Pow(10, (winner.RankELO - loser.RankELO) / 400));

        // Update ELO
        double winnerNewElo = winner.RankELO + K * (1 - expectedWinner);
        double loserNewElo = loser.RankELO + K * (0 - expectedLoser);

        winner.RankELO = Math.Max(100, winnerNewElo); // Minimum ELO = 100
        loser.RankELO = Math.Max(100, loserNewElo);
        
        match.EloChange = K * (1 - expectedWinner);

        _unitOfWork.Members.Update(winner);
        _unitOfWork.Members.Update(loser);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Thêm set cho trận đấu
    /// </summary>
    [HttpPost("{id}/sets")]
    [Authorize(Roles = "Referee,Admin")]
    public async Task<ActionResult<ApiResponse<MatchSetDto>>> AddMatchSet(int id, [FromBody] CreateMatchSetDto dto)
    {
        try
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null)
                return NotFound(ApiResponse<MatchSetDto>.ErrorResponse("Match not found"));

            var winningSide = dto.Team1Score > dto.Team2Score ? 1 : 2;

            var matchSet = new MatchSet
            {
                MatchId = id,
                SetNumber = dto.SetNumber,
                Team1Score = dto.Team1Score,
                Team2Score = dto.Team2Score,
                IsFinalSet = dto.IsFinalSet,
                WinningSide = winningSide
            };

            // TODO: Add to repository when MatchSets repository is implemented
            // await _unitOfWork.MatchSets.AddAsync(matchSet);
            // await _unitOfWork.CompleteAsync();

            var resultDto = new MatchSetDto
            {
                Id = matchSet.Id,
                MatchId = matchSet.MatchId,
                SetNumber = matchSet.SetNumber,
                Team1Score = matchSet.Team1Score,
                Team2Score = matchSet.Team2Score,
                IsFinalSet = matchSet.IsFinalSet,
                WinningSide = matchSet.WinningSide
            };

            return Ok(ApiResponse<MatchSetDto>.SuccessResponse(resultDto, "Set added successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MatchSetDto>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Cập nhật điểm của set
    /// </summary>
    [HttpPut("sets/{setId}")]
    [Authorize(Roles = "Referee,Admin")]
    public async Task<ActionResult<ApiResponse<MatchSetDto>>> UpdateMatchSet(int setId, [FromBody] CreateMatchSetDto dto)
    {
        try
        {
            // TODO: Implement when repository is ready
            return Ok(ApiResponse<MatchSetDto>.SuccessResponse(null, "Set updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MatchSetDto>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Xóa set
    /// </summary>
    [HttpDelete("sets/{setId}")]
    [Authorize(Roles = "Referee,Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMatchSet(int setId)
    {
        try
        {
            // TODO: Implement when repository is ready
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Set deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}"));
        }
    }
}

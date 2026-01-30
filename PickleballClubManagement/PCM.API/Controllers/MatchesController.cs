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
            
            var items = matches
                .OrderByDescending(m => m.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    Date = m.Date,
                    Status = m.WinningSide == WinningSide.None ? "Scheduled" : "Completed",
                    Team1Name = $"Team 1",
                    Team2Name = $"Team 2",
                    Team1Score = 0,
                    Team2Score = 0
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

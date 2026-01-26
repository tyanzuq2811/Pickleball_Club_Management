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
}

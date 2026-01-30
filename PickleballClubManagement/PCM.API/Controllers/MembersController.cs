using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Members;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// Lấy danh sách tất cả hội viên
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<PagedResult<MemberDto>>>> GetAllMembers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _memberService.GetAllAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy thông tin hội viên theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> GetMemberById(int id)
    {
        var result = await _memberService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Lấy thông tin hội viên hiện tại
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> GetCurrentMember()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _memberService.GetByUserIdAsync(userId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Cập nhật thông tin hội viên
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> UpdateMember(int id, [FromBody] MemberUpdateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var currentMember = await _memberService.GetByUserIdAsync(userId);
        if (currentMember?.Data == null)
            return Unauthorized();

        // Chỉ Admin hoặc chính chủ mới được cập nhật
        if (currentMember.Data.Id != id && !User.IsInRole("Admin"))
            return Forbid();

        var result = await _memberService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tạo hội viên mới (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MemberDto>>> CreateMember([FromBody] MemberCreateDto dto)
    {
        var result = await _memberService.CreateAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Xóa hội viên (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMember(int id)
    {
        var result = await _memberService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy BXH Top Ranking
    /// </summary>
    [HttpGet("top-ranking")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<MemberDto>>>> GetTopRanking([FromQuery] int limit = 10)
    {
        var result = await _memberService.GetTopRankingAsync(limit);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy số lượng hội viên (public endpoint)
    /// </summary>
    [HttpGet("count")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<int>>> GetMembersCount()
    {
        var result = await _memberService.GetAllAsync(1, 1);
        if (!result.Success) return BadRequest(result);
        
        return Ok(ApiResponse<int>.SuccessResponse(result.Data?.TotalCount ?? 0));
    }
}

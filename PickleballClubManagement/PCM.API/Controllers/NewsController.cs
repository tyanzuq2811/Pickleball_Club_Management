using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.News;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;
    private readonly IMemberService _memberService;

    public NewsController(INewsService newsService, IMemberService memberService)
    {
        _newsService = newsService;
        _memberService = memberService;
    }

    /// <summary>
    /// Lấy danh sách tin tức
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<NewsDto>>>> GetAllNews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _newsService.GetAllAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy tin tức được ghim
    /// </summary>
    [HttpGet("pinned")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<NewsDto>>>> GetPinnedNews()
    {
        var result = await _newsService.GetPinnedAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy tin tức theo ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<NewsDto>>> GetNewsById(int id)
    {
        var result = await _newsService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Tạo tin tức mới
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<NewsDto>>> CreateNews([FromBody] NewsCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member?.Data == null)
            return Unauthorized(ApiResponse<NewsDto>.ErrorResponse("Không tìm thấy thông tin thành viên"));

        var result = await _newsService.CreateAsync(dto, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật tin tức
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<NewsDto>>> UpdateNews(int id, [FromBody] NewsUpdateDto dto)
    {
        var result = await _newsService.UpdateAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Xóa tin tức
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteNews(int id)
    {
        var result = await _newsService.DeleteAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
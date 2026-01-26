using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Lấy danh sách thông báo của user hiện tại
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetNotifications()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Tạm thời trả về empty list vì chưa implement service
        return Ok(ApiResponse<List<object>>.SuccessResponse(new List<object>()));
    }

    /// <summary>
    /// Đánh dấu thông báo đã đọc
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(int id)
    {
        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    /// <summary>
    /// Đánh dấu tất cả đã đọc
    /// </summary>
    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead()
    {
        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}

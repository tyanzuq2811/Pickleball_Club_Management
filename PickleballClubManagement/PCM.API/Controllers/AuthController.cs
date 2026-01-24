using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Auth;
using PCM.Application.DTOs.Common;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Làm mới token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Thu hồi token
    /// </summary>
    [HttpPost("revoke-token")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<bool>.ErrorResponse("Không tìm thấy thông tin người dùng"));

        var result = await _authService.RevokeTokenAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<UserInfoDto>.ErrorResponse("Không tìm thấy thông tin người dùng"));

        var result = await _authService.GetCurrentUserAsync(userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
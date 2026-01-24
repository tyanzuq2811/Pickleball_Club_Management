using PCM.Application.DTOs.Auth;
using PCM.Application.DTOs.Common;

namespace PCM.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<bool>> RevokeTokenAsync(string userId);
    Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(string userId);
}

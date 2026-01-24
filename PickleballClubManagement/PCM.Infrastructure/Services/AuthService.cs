using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using PCM.Application.DTOs.Auth;
using PCM.Application.DTOs.Common;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;

namespace PCM.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IActivityLogService _activityLogService;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IActivityLogService activityLogService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _activityLogService = activityLogService;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            
            if (!result.Succeeded)
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");

            var member = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.UserId == user.Id);
            
            if (member == null)
                return ApiResponse<AuthResponseDto>.ErrorResponse("Member profile not found");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user, roles.ToList());
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                User = new UserInfoDto
                {
                    UserId = user.Id,
                    MemberId = member.Id,
                    Email = user.Email!,
                    FullName = member.FullName,
                    Roles = roles.ToList(),
                    WalletBalance = member.WalletBalance,
                    RankELO = member.RankELO
                }
            };

            await _activityLogService.LogAsync(user.Id, "LOGIN", "User logged in successfully");

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse($"Login failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            
            if (existingUser != null)
                return ApiResponse<AuthResponseDto>.ErrorResponse("Email already exists");

            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<AuthResponseDto>.ErrorResponse("Registration failed", errors);
            }

            await _userManager.AddToRoleAsync(user, "Member");

            var member = new Member
            {
                UserId = user.Id,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                JoinDate = DateTime.UtcNow,
                RankELO = 1200,
                WalletBalance = 0,
                IsActive = true
            };

            await _unitOfWork.Members.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(user.Id, "REGISTER", "New user registered");

            return await LoginAsync(new LoginRequestDto 
            { 
                Email = request.Email, 
                Password = request.Password 
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthResponseDto>.ErrorResponse($"Registration failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var storedToken = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (storedToken == null)
            return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid refresh token");

        if (storedToken.ExpiryDate < DateTime.UtcNow)
            return ApiResponse<AuthResponseDto>.ErrorResponse("Refresh token has expired");

        if (storedToken.IsUsed)
            return ApiResponse<AuthResponseDto>.ErrorResponse("Refresh token has been used");

        if (storedToken.IsRevoked)
            return ApiResponse<AuthResponseDto>.ErrorResponse("Refresh token has been revoked");

        storedToken.IsUsed = true;
        _unitOfWork.RefreshTokens.Update(storedToken);
        await _unitOfWork.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.UserId == user.Id);
        var roles = await _userManager.GetRolesAsync(user);
        
        var newToken = await GenerateJwtToken(user, roles.ToList());
        var newRefreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            JwtId = newToken.Id,
            IsUsed = false,
            IsRevoked = false,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newToken),
            RefreshToken = newRefreshToken,
            Expiration = newToken.ValidTo,
            User = new UserInfoDto { UserId = user.Id, MemberId = member?.Id ?? 0, Email = user.Email!, FullName = member?.FullName ?? "", Roles = roles.ToList() }
        });
    }

    public async Task<ApiResponse<bool>> RevokeTokenAsync(string userId)
    {
        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");

            var member = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.UserId == userId);
            if (member == null)
                return ApiResponse<UserInfoDto>.ErrorResponse("Member profile not found");

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = new UserInfoDto
            {
                UserId = user.Id,
                MemberId = member.Id,
                Email = user.Email!,
                FullName = member.FullName,
                Roles = roles.ToList(),
                WalletBalance = member.WalletBalance,
                RankELO = member.RankELO
            };

            return ApiResponse<UserInfoDto>.SuccessResponse(userInfo);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserInfoDto>.ErrorResponse($"Get user info failed: {ex.Message}");
        }
    }

    private async Task<JwtSecurityToken> GenerateJwtToken(IdentityUser user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"])),
            signingCredentials: creds
        );

        return token;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
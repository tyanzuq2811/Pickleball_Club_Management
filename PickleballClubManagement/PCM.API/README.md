# Backend PCM - Pickleball Club Management System

## TỔNG QUAN ĐÃ HOÀN THÀNH

### ✅ Domain Layer (PCM.Domain)
- **Entities**: Tất cả 15 entities với tên bảng prefix 189_
  - Member, RefreshToken, News
  - TransactionCategory, WalletTransaction, Transaction
  - Court, Booking
  - Tournament, Participant, TournamentMatch, Match, MatchScore
  - Notification, ActivityLog
- **Enums**: 12 enums đầy đủ
- **Interfaces**: IRepository<T>, IUnitOfWork

### ✅ Infrastructure Layer (PCM.Infrastructure)
- **ApplicationDbContext**: Cấu hình EF Core, Identity, relationships, indexes
- **Repository Pattern**: Repository<T> generic implementation
- **UnitOfWork**: Quản lý transactions
- **RedisService**: Caching và leaderboard
- **DbInitializer**: Seed data (roles, users, categories, courts)

### ✅ Application Layer (PCM.Application)
- **DTOs**: Auth, Members, Bookings, Courts, Tournaments, Transactions, Wallet, News
- **Service Interfaces**: 8 interfaces (IAuthService, IMemberService, IBookingService, etc.)
- **Services**: MemberService (mẫu)
- **AutoMapper**: MappingProfile configuration

### ✅ API Layer (PCM.API)
- **appsettings.json**: Connection string đến TYANZUQ-2811\TYANZUQ
- **Program.cs**: Cấu hình đầy đủ (JWT, Identity, Redis, Hangfire, SignalR, CORS, Swagger)
- **Middleware**: ExceptionMiddleware
- **Controllers**: Đã có sẵn (cần kiểm tra và bổ sung)

## ❌ LỖI CẦN SỬA

### 1. AuthService bị thiếu trong Infrastructure
**File**: `PCM.Infrastructure/Services/AuthService.cs`

Tạo lại file AuthService:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
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

            // Save refresh token
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

            // Create Member profile
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

            // Login after registration
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
        // Implement refresh token logic
        return ApiResponse<AuthResponseDto>.ErrorResponse("Not implemented");
    }

    public async Task<ApiResponse<bool>> RevokeTokenAsync(string userId)
    {
        // Implement revoke token logic
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
```

### 2. Update Program.cs

Sửa lại dòng register AuthService:

```csharp
// Register Application Services
builder.Services.AddScoped<IAuthService, PCM.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
```

### 3. Xóa file AuthService từ Application/Services (nếu còn)

```powershell
Remove-Item PCM.Application\Services\AuthService.cs -Force -ErrorAction SilentlyContinue
```

## 🚀 CHẠY MIGRATION

Sau khi sửa lỗi trên, chạy các lệnh:

```powershell
# Restore packages
cd d:\FullStack\Test2\PickleballClubManagement
dotnet restore

# Build
dotnet build

# Tạo migration
cd PCM.API
dotnet ef migrations add InitialCreate --project ..\PCM.Infrastructure --startup-project .

# Update database
dotnet ef database update --project ..\PCM.Infrastructure --startup-project .

# Run API
dotnet run
```

## 📝 CÁC SERVICES CẦN BỔ SUNG

Tạo các services sau trong `PCM.Application/Services`:

1. **BookingService**: Quản lý đặt sân, recurring booking, conflict check
2. **CourtService**: CRUD courts
3. **NewsService**: CRUD news với Redis cache
4. **TournamentService**: Tạo giải, join, auto divide teams, update scores
5. **TransactionService**: Quản lý tài chính CLB
6. **WalletService**: Nạp tiền, trừ tiền, approve deposit

## 🔄 SIGNALR HUBS

Tạo các hubs trong `PCM.API/Hubs`:

1. **NotificationHub**: Thông báo real-time
2. **ScoreboardHub**: Cập nhật tỷ số trận đấu
3. **BookingHub**: Cập nhật trạng thái sân

## ⏰ HANGFIRE JOBS

Tạo trong `PCM.API/Jobs`:

1. **CancelPendingBookingsJob**: Hủy booking pending quá 15 phút
2. **DailyReportJob**: Báo cáo doanh thu cuối ngày
3. **UpdateLeaderboardJob**: Cập nhật BXH

## 🔧 LƯU Ý

- **Redis**: Cần cài đặt Redis (có thể dùng Docker: `docker run -d -p 6379:6379 redis`)
- **SQL Server**: Đảm bảo server `TYANZUQ-2811\TYANZUQ` đang chạy
- **Connection String**: Đã cấu hình trong appsettings.json
- **JWT Secret Key**: Đã có trong appsettings.json (thay đổi trong production)

## 📚 TESTING

Sau khi run API, truy cập:
- Swagger UI: https://localhost:7xxx/swagger
- Hangfire Dashboard: https://localhost:7xxx/hangfire

Default credentials:
- Admin: admin@pcm.com / Admin@123
- Member1: nguyen.vana@pcm.com / Member@123
- Member2: tran.thib@pcm.com / Member@123

## 🎯 NEXT STEPS

1. Sửa lỗi AuthService
2. Build & Run migration
3. Test Auth endpoints
4. Tạo các services còn lại
5. Tạo SignalR hubs
6. Tạo Hangfire jobs
7. Test toàn bộ API với Swagger
8. Xây dựng Frontend (Vue.js)

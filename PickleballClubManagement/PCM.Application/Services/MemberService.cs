using Microsoft.EntityFrameworkCore;
using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Members;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;

namespace PCM.Application.Services;

public class MemberService : IMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public MemberService(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse<MemberDto>> GetByIdAsync(int id)
    {
        try
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            
            if (member == null)
                return ApiResponse<MemberDto>.ErrorResponse("Member not found");

            var dto = new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                DateOfBirth = member.DateOfBirth,
                JoinDate = member.JoinDate,
                RankELO = member.RankELO,
                WalletBalance = member.WalletBalance,
                AvatarUrl = member.AvatarUrl,
                IsActive = member.IsActive,
                TotalMatches = member.TotalMatches,
                WinMatches = member.WinMatches
            };

            return ApiResponse<MemberDto>.SuccessResponse(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<MemberDto>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<MemberDto>> GetByUserIdAsync(string userId)
    {
        try
        {
            var member = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.UserId == userId);
            
            if (member == null)
                return ApiResponse<MemberDto>.ErrorResponse("Member not found");

            var dto = new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                DateOfBirth = member.DateOfBirth,
                JoinDate = member.JoinDate,
                RankELO = member.RankELO,
                WalletBalance = member.WalletBalance,
                AvatarUrl = member.AvatarUrl,
                IsActive = member.IsActive,
                TotalMatches = member.TotalMatches,
                WinMatches = member.WinMatches
            };

            return ApiResponse<MemberDto>.SuccessResponse(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<MemberDto>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<MemberDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var members = await _unitOfWork.Members.GetAllAsync();
            var totalCount = members.Count();

            var pagedMembers = members
                .OrderBy(m => m.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(member => new MemberDto
                {
                    Id = member.Id,
                    UserId = member.UserId,
                    FullName = member.FullName,
                    Email = member.Email,
                    PhoneNumber = member.PhoneNumber,
                    DateOfBirth = member.DateOfBirth,
                    JoinDate = member.JoinDate,
                    RankELO = member.RankELO,
                    WalletBalance = member.WalletBalance,
                    AvatarUrl = member.AvatarUrl,
                    IsActive = member.IsActive,
                    TotalMatches = member.TotalMatches,
                    WinMatches = member.WinMatches
                })
                .ToList();

            var result = new PagedResult<MemberDto>
            {
                Items = pagedMembers,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return ApiResponse<PagedResult<MemberDto>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResult<MemberDto>>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<MemberRankingDto>>> GetTopRankingAsync(int limit = 10)
    {
        try
        {
            // Try to get from Redis cache first
            try
            {
                var cacheKey = $"leaderboard:top:{limit}";
                var cachedRanking = await _redisService.GetAsync<List<MemberRankingDto>>(cacheKey);

                if (cachedRanking != null && cachedRanking.Any())
                    return ApiResponse<List<MemberRankingDto>>.SuccessResponse(cachedRanking);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis cache read failed: {ex.Message}");
            }

            // If not in cache, get from database
            var members = await _unitOfWork.Members.GetAllAsync();
            var ranking = members
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.RankELO)
                .Take(limit)
                .Select((member, index) => new MemberRankingDto
                {
                    Rank = index + 1,
                    MemberId = member.Id,
                    FullName = member.FullName,
                    RankELO = member.RankELO,
                    TotalMatches = member.TotalMatches,
                    WinMatches = member.WinMatches,
                    WinRate = member.TotalMatches > 0 ? (double)member.WinMatches / member.TotalMatches * 100 : 0,
                    AvatarUrl = member.AvatarUrl
                })
                .ToList();

            // Cache for 5 minutes
            try
            {
                var cacheKey = $"leaderboard:top:{limit}";
                await _redisService.SetAsync(cacheKey, ranking, TimeSpan.FromMinutes(5));

                // Also update Redis Sorted Set for leaderboard
                foreach (var item in ranking)
                {
                    await _redisService.SortedSetAddAsync("leaderboard:elo", item.MemberId.ToString(), item.RankELO);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis cache write failed: {ex.Message}");
            }

            return ApiResponse<List<MemberRankingDto>>.SuccessResponse(ranking);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<MemberRankingDto>>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<MemberDto>> UpdateAsync(int id, MemberUpdateDto dto)
    {
        try
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            
            if (member == null)
                return ApiResponse<MemberDto>.ErrorResponse("Member not found");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                member.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                member.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                member.PhoneNumber = dto.PhoneNumber;

            if (dto.DateOfBirth.HasValue)
                member.DateOfBirth = dto.DateOfBirth;

            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
                member.AvatarUrl = dto.AvatarUrl;

            // Admin có thể cập nhật các trường này
            if (dto.RankELO.HasValue)
                member.RankELO = dto.RankELO.Value;

            if (dto.WalletBalance.HasValue)
                member.WalletBalance = dto.WalletBalance.Value;

            if (dto.IsActive.HasValue)
                member.IsActive = dto.IsActive.Value;

            member.ModifiedDate = DateTime.UtcNow;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                DateOfBirth = member.DateOfBirth,
                JoinDate = member.JoinDate,
                RankELO = member.RankELO,
                WalletBalance = member.WalletBalance,
                AvatarUrl = member.AvatarUrl,
                IsActive = member.IsActive,
                TotalMatches = member.TotalMatches,
                WinMatches = member.WinMatches
            };

            return ApiResponse<MemberDto>.SuccessResponse(resultDto, "Member updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MemberDto>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<MemberDto>> CreateAsync(MemberCreateDto dto)
    {
        try
        {
            // Kiểm tra email đã tồn tại chưa
            var existingMember = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.Email == dto.Email);
            if (existingMember != null)
                return ApiResponse<MemberDto>.ErrorResponse("Email đã được sử dụng");

            var member = new Member
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                RankELO = dto.RankELO > 0 ? dto.RankELO : 1200,
                WalletBalance = dto.WalletBalance,
                IsActive = true,
                JoinDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                UserId = Guid.NewGuid().ToString() // Generate unique ID, sẽ được cập nhật khi user đăng ký
            };

            await _unitOfWork.Members.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                FullName = member.FullName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                DateOfBirth = member.DateOfBirth,
                JoinDate = member.JoinDate,
                RankELO = member.RankELO,
                WalletBalance = member.WalletBalance,
                AvatarUrl = member.AvatarUrl,
                IsActive = member.IsActive,
                TotalMatches = member.TotalMatches,
                WinMatches = member.WinMatches
            };

            return ApiResponse<MemberDto>.SuccessResponse(resultDto, "Tạo hội viên thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<MemberDto>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        try
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            
            if (member == null)
                return ApiResponse<bool>.ErrorResponse("Không tìm thấy hội viên");

            // Soft delete - chỉ đánh dấu là không hoạt động
            member.IsActive = false;
            member.ModifiedDate = DateTime.UtcNow;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();

            // Xóa khỏi Redis leaderboard
            try
            {
                await _redisService.SortedSetRemoveAsync("leaderboard:elo", id.ToString());
                await _redisService.DeleteAsync("leaderboard:top:10");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis delete failed: {ex.Message}");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Xóa hội viên thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> UpdateRankELOAsync(int memberId, double eloChange)
    {
        try
        {
            var member = await _unitOfWork.Members.GetByIdAsync(memberId);
            
            if (member == null)
                return ApiResponse<bool>.ErrorResponse("Member not found");

            member.RankELO += eloChange;
            member.ModifiedDate = DateTime.UtcNow;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();

            // Update Redis leaderboard
            try
            {
                await _redisService.SortedSetAddAsync("leaderboard:elo", memberId.ToString(), member.RankELO);
                await _redisService.DeleteAsync("leaderboard:top:10");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis update failed: {ex.Message}");
            }

            return ApiResponse<bool>.SuccessResponse(true, "Rank ELO updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
        }
    }
}

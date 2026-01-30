using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Members;

namespace PCM.Application.Interfaces;

public interface IMemberService
{
    Task<ApiResponse<MemberDto>> GetByIdAsync(int id);
    Task<ApiResponse<MemberDto>> GetByUserIdAsync(string userId);
    Task<ApiResponse<PagedResult<MemberDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<List<MemberRankingDto>>> GetTopRankingAsync(int limit = 10);
    Task<ApiResponse<MemberDto>> CreateAsync(MemberCreateDto dto);
    Task<ApiResponse<MemberDto>> UpdateAsync(int id, MemberUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<bool>> UpdateRankELOAsync(int memberId, double eloChange);
}

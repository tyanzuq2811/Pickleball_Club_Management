using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Courts;

namespace PCM.Application.Interfaces;

public interface ICourtService
{
    Task<ApiResponse<CourtDto>> GetByIdAsync(int id);
    Task<ApiResponse<List<CourtDto>>> GetAllAsync();
    Task<ApiResponse<List<CourtDto>>> GetActiveAsync();
    Task<ApiResponse<CourtDto>> CreateAsync(CourtCreateDto dto);
    Task<ApiResponse<CourtDto>> UpdateAsync(int id, CourtUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

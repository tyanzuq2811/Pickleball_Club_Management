using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.News;

namespace PCM.Application.Interfaces;

public interface INewsService
{
    Task<ApiResponse<NewsDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResult<NewsDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<List<NewsDto>>> GetPinnedAsync();
    Task<ApiResponse<NewsDto>> CreateAsync(NewsCreateDto dto, string createdBy);
    Task<ApiResponse<NewsDto>> UpdateAsync(int id, NewsUpdateDto dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.News;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;
using StackExchange.Redis;

namespace PCM.Application.Services;

public class NewsService : INewsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;
    private const string PinnedNewsCacheKey = "news:pinned";

    public NewsService(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse<NewsDto>> GetByIdAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null) return ApiResponse<NewsDto>.ErrorResponse("News not found");
        
        return ApiResponse<NewsDto>.SuccessResponse(new NewsDto 
        { 
            Id = news.Id, Title = news.Title, Content = news.Content, 
            IsPinned = news.IsPinned, CreatedDate = news.CreatedDate, CreatedBy = news.CreatedBy 
        });
    }

    public async Task<ApiResponse<PagedResult<NewsDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var list = await _unitOfWork.News.GetAllAsync();
        var total = list.Count();
        var items = list.OrderByDescending(n => n.CreatedDate)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(n => new NewsDto 
            { 
                Id = n.Id, Title = n.Title, Content = n.Content, 
                IsPinned = n.IsPinned, CreatedDate = n.CreatedDate, CreatedBy = n.CreatedBy 
            }).ToList();

        return ApiResponse<PagedResult<NewsDto>>.SuccessResponse(new PagedResult<NewsDto> 
        { 
            Items = items, TotalCount = total, PageNumber = pageNumber, PageSize = pageSize 
        });
    }

    public async Task<ApiResponse<List<NewsDto>>> GetPinnedAsync()
    {
        // Cache Strategy
        var cached = await _redisService.GetAsync<List<NewsDto>>(PinnedNewsCacheKey);
        if (cached != null) return ApiResponse<List<NewsDto>>.SuccessResponse(cached);

        var list = await _unitOfWork.News.FindAsync(n => n.IsPinned);
        var dtos = list.OrderByDescending(n => n.CreatedDate)
            .Select(n => new NewsDto 
            { 
                Id = n.Id, Title = n.Title, Content = n.Content, 
                IsPinned = n.IsPinned, CreatedDate = n.CreatedDate, CreatedBy = n.CreatedBy 
            }).ToList();

        await _redisService.SetAsync(PinnedNewsCacheKey, dtos, TimeSpan.FromMinutes(30));
        return ApiResponse<List<NewsDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<NewsDto>> CreateAsync(NewsCreateDto dto, string createdBy)
    {
        var news = new News { Title = dto.Title, Content = dto.Content, IsPinned = dto.IsPinned, CreatedBy = createdBy, CreatedDate = DateTime.UtcNow };
        await _unitOfWork.News.AddAsync(news);
        await _unitOfWork.SaveChangesAsync();
        
        if (dto.IsPinned) await _redisService.DeleteAsync(PinnedNewsCacheKey);

        return ApiResponse<NewsDto>.SuccessResponse(new NewsDto { Id = news.Id, Title = news.Title }, "News created");
    }

    public async Task<ApiResponse<NewsDto>> UpdateAsync(int id, NewsUpdateDto dto)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null) return ApiResponse<NewsDto>.ErrorResponse("News not found");

        if (dto.Title != null) news.Title = dto.Title;
        if (dto.Content != null) news.Content = dto.Content;
        if (dto.IsPinned.HasValue) news.IsPinned = dto.IsPinned.Value;
        news.ModifiedDate = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        await _redisService.DeleteAsync(PinnedNewsCacheKey);
        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var news = await _unitOfWork.News.GetByIdAsync(id);
        if (news == null) return ApiResponse<bool>.ErrorResponse("News not found");
        
        _unitOfWork.News.Remove(news);
        await _unitOfWork.SaveChangesAsync();
        await _redisService.DeleteAsync(PinnedNewsCacheKey);
        
        return ApiResponse<bool>.SuccessResponse(true, "News deleted");
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Domain.Interfaces;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<ApiResponse<List<SearchResultDto>>>> SearchBookings(
        [FromQuery] string? keyword,
        [FromQuery] int? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? courtId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        try
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var query = bookings.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => 
                    (b.Court != null && b.Court.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (b.Member != null && b.Member.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
            }

            if (status.HasValue)
                query = query.Where(b => (int)b.Status == status.Value);

            if (startDate.HasValue)
                query = query.Where(b => b.StartTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(b => b.EndTime <= endDate.Value);

            if (courtId.HasValue)
                query = query.Where(b => b.CourtId == courtId.Value);

            if (minPrice.HasValue)
                query = query.Where(b => b.TotalPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(b => b.TotalPrice <= maxPrice.Value);

            var results = query.ToList().Select(b => new SearchResultDto
            {
                Id = b.Id,
                Title = $"Booking #{b.Id} - {(b.Court != null ? b.Court.Name : "N/A")}",
                Description = $"{(b.Member != null ? b.Member.FullName : "N/A")} - {b.StartTime:dd/MM/yyyy HH:mm} - {b.TotalPrice:N0} VNĐ",
                Type = "booking"
            }).ToList();

            return Ok(ApiResponse<List<SearchResultDto>>.SuccessResponse(results));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SearchResultDto>>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    [HttpGet("members")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<List<SearchResultDto>>>> SearchMembers(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] string? phone,
        [FromQuery] bool? isActive,
        [FromQuery] decimal? minBalance,
        [FromQuery] decimal? maxBalance)
    {
        try
        {
            var members = await _unitOfWork.Members.GetAllAsync();
            var query = members.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(m => m.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(m => m.Email.Contains(email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(m => m.PhoneNumber.Contains(phone));

            if (minBalance.HasValue)
                query = query.Where(m => m.WalletBalance >= minBalance.Value);

            if (maxBalance.HasValue)
                query = query.Where(m => m.WalletBalance <= maxBalance.Value);

            var results = query.Select(m => new SearchResultDto
            {
                Id = m.Id,
                Title = m.FullName,
                Description = $"{m.Email} - {m.PhoneNumber} - Ví: {m.WalletBalance:N0} VNĐ",
                Type = "member"
            }).ToList();

            return Ok(ApiResponse<List<SearchResultDto>>.SuccessResponse(results));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SearchResultDto>>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    [HttpGet("transactions")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<List<SearchResultDto>>>> SearchTransactions(
        [FromQuery] int? type,
        [FromQuery] int? categoryId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount)
    {
        try
        {
            var transactions = await _unitOfWork.Transactions.GetAllAsync();
            var query = transactions.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            if (minAmount.HasValue)
                query = query.Where(t => Math.Abs(t.Amount) >= minAmount.Value);

            if (maxAmount.HasValue)
                query = query.Where(t => Math.Abs(t.Amount) <= maxAmount.Value);

            var results = query.ToList().Select(t => new SearchResultDto
            {
                Id = t.Id,
                Title = $"Giao dịch #{t.Id} - {(t.Category != null ? t.Category.Name : "N/A")}",
                Description = $"{t.Date:dd/MM/yyyy} - {t.Amount:N0} VNĐ - {t.Description}",
                Type = "transaction"
            }).ToList();

            return Ok(ApiResponse<List<SearchResultDto>>.SuccessResponse(results));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SearchResultDto>>.ErrorResponse($"Error: {ex.Message}"));
        }
    }

    [HttpGet("tournaments")]
    public async Task<ActionResult<ApiResponse<List<SearchResultDto>>>> SearchTournaments(
        [FromQuery] string? name,
        [FromQuery] int? type,
        [FromQuery] int? status,
        [FromQuery] DateTime? startDate)
    {
        try
        {
            var tournaments = await _unitOfWork.Tournaments.GetAllAsync();
            var query = tournaments.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(t => t.Title.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (type.HasValue)
                query = query.Where(t => (int)t.Type == type.Value);

            if (status.HasValue)
                query = query.Where(t => (int)t.Status == status.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.StartDate >= startDate.Value);

            var results = query.Select(t => new SearchResultDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = $"{t.Type} - {t.Status} - {t.StartDate:dd/MM/yyyy}",
                Type = "tournament"
            }).ToList();

            return Ok(ApiResponse<List<SearchResultDto>>.SuccessResponse(results));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<SearchResultDto>>.ErrorResponse($"Error: {ex.Message}"));
        }
    }
}

public class SearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

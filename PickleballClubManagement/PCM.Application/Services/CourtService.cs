using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Courts;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;

namespace PCM.Application.Services;

public class CourtService : ICourtService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourtService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CourtDto>> GetByIdAsync(int id)
    {
        var court = await _unitOfWork.Courts.GetByIdAsync(id);
        if (court == null) return ApiResponse<CourtDto>.ErrorResponse("Court not found");
        return ApiResponse<CourtDto>.SuccessResponse(new CourtDto { Id = court.Id, Name = court.Name, IsActive = court.IsActive, Description = court.Description });
    }

    public async Task<ApiResponse<List<CourtDto>>> GetAllAsync()
    {
        var list = await _unitOfWork.Courts.GetAllAsync();
        var dtos = list.Select(c => new CourtDto { Id = c.Id, Name = c.Name, IsActive = c.IsActive, Description = c.Description }).ToList();
        return ApiResponse<List<CourtDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<List<CourtDto>>> GetActiveAsync()
    {
        var list = await _unitOfWork.Courts.FindAsync(c => c.IsActive);
        var dtos = list.Select(c => new CourtDto { Id = c.Id, Name = c.Name, IsActive = c.IsActive, Description = c.Description }).ToList();
        return ApiResponse<List<CourtDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<CourtDto>> CreateAsync(CourtCreateDto dto)
    {
        var court = new Court { Name = dto.Name, Description = dto.Description, IsActive = true, CreatedDate = DateTime.UtcNow };
        await _unitOfWork.Courts.AddAsync(court);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<CourtDto>.SuccessResponse(new CourtDto { Id = court.Id, Name = court.Name }, "Court created");
    }

    public async Task<ApiResponse<CourtDto>> UpdateAsync(int id, CourtUpdateDto dto)
    {
        var court = await _unitOfWork.Courts.GetByIdAsync(id);
        if (court == null) return ApiResponse<CourtDto>.ErrorResponse("Court not found");

        if (dto.Name != null) court.Name = dto.Name;
        if (dto.Description != null) court.Description = dto.Description;
        if (dto.IsActive.HasValue) court.IsActive = dto.IsActive.Value;

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var court = await _unitOfWork.Courts.GetByIdAsync(id);
        if (court == null) return ApiResponse<bool>.ErrorResponse("Court not found");
        
        // Check dependencies
        var hasBookings = await _unitOfWork.Bookings.AnyAsync(b => b.CourtId == id);
        if (hasBookings) return ApiResponse<bool>.ErrorResponse("Cannot delete court with existing bookings");

        _unitOfWork.Courts.Remove(court);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Court deleted");
    }
}
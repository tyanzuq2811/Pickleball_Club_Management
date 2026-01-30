using PCM.Application.DTOs.Bookings;
using PCM.Application.DTOs.Common;

namespace PCM.Application.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<PagedResult<BookingDto>>> GetAllAsync(int pageNumber, int pageSize);
    Task<ApiResponse<BookingDto>> GetByIdAsync(int id);
    Task<ApiResponse<List<BookingDto>>> GetMemberBookingsAsync(int memberId);
    Task<ApiResponse<BookingDto>> CreateAsync(BookingCreateDto dto, int memberId);
    Task<ApiResponse<List<BookingDto>>> CreateRecurringAsync(RecurringBookingDto dto, int memberId);
    Task<ApiResponse<bool>> CancelAsync(int id, int memberId);
    Task<ApiResponse<bool>> PayBookingAsync(int bookingId, int memberId);
    Task<ApiResponse<bool>> DeleteAsync(int bookingId, int memberId);
    Task<ApiResponse<bool>> CheckInAsync(int bookingId, int memberId);
    Task<ApiResponse<List<AvailableSlotDto>>> GetAvailableSlotsAsync(int courtId, DateTime date);
    Task ScanAndCancelExpiredBookingsAsync();
}
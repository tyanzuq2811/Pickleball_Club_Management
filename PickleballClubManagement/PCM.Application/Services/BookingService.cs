using Microsoft.EntityFrameworkCore;
using PCM.Application.DTOs.Bookings;
using PCM.Application.DTOs.Common;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Enums;
using PCM.Domain.Interfaces;

namespace PCM.Application.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWalletService _walletService;
    private readonly INotificationService _notificationService;

    public BookingService(
        IUnitOfWork unitOfWork, 
        IWalletService walletService,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _walletService = walletService;
        _notificationService = notificationService;
    }

    public async Task<ApiResponse<BookingDto>> CreateAsync(BookingCreateDto dto, int memberId)
    {
        try
        {
            // 1. Validate Time
            if (dto.StartTime < DateTime.UtcNow)
                return ApiResponse<BookingDto>.ErrorResponse("Cannot book in the past");
            
            if (dto.EndTime <= dto.StartTime)
                return ApiResponse<BookingDto>.ErrorResponse("End time must be after start time");

            // 2. Check Conflict
            var isConflict = await _unitOfWork.Bookings.AnyAsync(b => 
                b.CourtId == dto.CourtId &&
                b.Status != BookingStatus.Cancelled &&
                ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                 (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                 (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

            if (isConflict)
                return ApiResponse<BookingDto>.ErrorResponse("Court is already booked in this time slot");

            // 3. Calculate Price (100k/hour)
            var duration = (dto.EndTime - dto.StartTime).TotalHours;
            var price = (decimal)duration * 100000m;

            // 4. Create Booking
            var booking = new Booking
            {
                CourtId = dto.CourtId,
                MemberId = memberId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalPrice = price,
                Status = BookingStatus.PendingPayment,
                Notes = dto.Notes,
                CreatedDate = DateTime.UtcNow
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            
            // 5. Auto Payment
            var paymentResult = await _walletService.PayBookingAsync(memberId, price, booking.Id.ToString());
            if (!paymentResult.Success)
            {
                return ApiResponse<BookingDto>.ErrorResponse($"Insufficient wallet balance: {paymentResult.Message}");
            }

            booking.Status = BookingStatus.Confirmed;
            await _unitOfWork.SaveChangesAsync();

            // Create Notification
            var notification = new Notification
            {
                MemberId = memberId,
                Title = "Booking Confirmed",
                Content = $"Your booking for {booking.CourtId} at {booking.StartTime} has been confirmed.",
                Type = NotificationType.Success,
                CreatedDate = DateTime.UtcNow
            };
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // Real-time: Notify all clients to update court status
            await _notificationService.BroadcastBookingUpdateAsync(booking.CourtId);

            // Return DTO (Simplified)
            return ApiResponse<BookingDto>.SuccessResponse(new BookingDto { Id = booking.Id, Status = booking.Status }, "Booking created successfully");
        }
        catch (DbUpdateConcurrencyException)
        {
            return ApiResponse<BookingDto>.ErrorResponse("Xung đột dữ liệu: Sân này vừa được người khác đặt. Vui lòng tải lại trang.");
        }
        catch (Exception ex)
        {
            return ApiResponse<BookingDto>.ErrorResponse($"Booking failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<BookingDto>>> GetAllAsync(int pageNumber, int pageSize)
    {
        var bookings = await _unitOfWork.Bookings.GetAllAsync();
        var members = await _unitOfWork.Members.GetAllAsync();
        var courts = await _unitOfWork.Courts.GetAllAsync();
        
        var memberDict = members.ToDictionary(m => m.Id, m => m.FullName);
        var courtDict = courts.ToDictionary(c => c.Id, c => c.Name);
        
        var total = bookings.Count();
        var items = bookings.OrderByDescending(b => b.StartTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingDto 
            { 
                Id = b.Id, 
                CourtId = b.CourtId,
                CourtName = courtDict.ContainsKey(b.CourtId) ? courtDict[b.CourtId] : "Unknown",
                MemberId = b.MemberId,
                MemberName = memberDict.ContainsKey(b.MemberId) ? memberDict[b.MemberId] : "Unknown",
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                TotalPrice = b.TotalPrice,
                Notes = b.Notes
            }).ToList();

        return ApiResponse<PagedResult<BookingDto>>.SuccessResponse(new PagedResult<BookingDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<BookingDto>> GetByIdAsync(int id)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return ApiResponse<BookingDto>.ErrorResponse("Booking not found");
        
        return ApiResponse<BookingDto>.SuccessResponse(new BookingDto 
        { 
            Id = booking.Id, 
            CourtId = booking.CourtId,
            MemberId = booking.MemberId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice
        });
    }

    public async Task<ApiResponse<List<BookingDto>>> GetMemberBookingsAsync(int memberId)
    {
        var bookings = await _unitOfWork.Bookings
            .FindAsync(b => b.MemberId == memberId);
        
        var court = await _unitOfWork.Courts.GetAllAsync();
        var courtDict = court.ToDictionary(c => c.Id, c => c.Name);
        
        var result = bookings
            .OrderByDescending(b => b.StartTime)
            .Select(b => new BookingDto 
            { 
                Id = b.Id, 
                CourtId = b.CourtId,
                CourtName = courtDict.ContainsKey(b.CourtId) ? courtDict[b.CourtId] : "Unknown",
                MemberId = b.MemberId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status,
                TotalPrice = b.TotalPrice
            }).ToList();

        return ApiResponse<List<BookingDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<BookingDto>>> CreateRecurringAsync(RecurringBookingDto dto, int memberId)
    {
        // Logic đơn giản: Loop qua các ngày và gọi CreateAsync
        var result = new List<BookingDto>();
        var currentDate = dto.StartDate;

        while (currentDate <= dto.EndDate)
        {
            if (dto.DaysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var start = currentDate.Date.Add(dto.StartTime);
                var end = currentDate.Date.Add(dto.EndTime);
                
                var createDto = new BookingCreateDto 
                { 
                    CourtId = dto.CourtId, 
                    StartTime = start, 
                    EndTime = end, 
                    Notes = dto.Notes 
                };

                var bookingRes = await CreateAsync(createDto, memberId);
                if (bookingRes.Success && bookingRes.Data != null)
                {
                    result.Add(bookingRes.Data);
                }
            }
            currentDate = currentDate.AddDays(1);
        }

        return ApiResponse<List<BookingDto>>.SuccessResponse(result, $"Created {result.Count} bookings");
    }

    public async Task<ApiResponse<bool>> CancelAsync(int id, int memberId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return ApiResponse<bool>.ErrorResponse("Booking not found");
        
        // Chỉ cho phép hủy nếu là chủ sở hữu hoặc admin (ở đây check chủ sở hữu)
        if (booking.MemberId != memberId) return ApiResponse<bool>.ErrorResponse("Unauthorized");

        if (booking.Status == BookingStatus.Cancelled) return ApiResponse<bool>.ErrorResponse("Already cancelled");

        booking.Status = BookingStatus.Cancelled;
        await _unitOfWork.SaveChangesAsync();
        
        // TODO: Hoàn tiền nếu cần (gọi WalletService.Refund)

        // Real-time: Notify all clients to update court status (unlock slot)
        await _notificationService.BroadcastBookingUpdateAsync(booking.CourtId);
        
        return ApiResponse<bool>.SuccessResponse(true, "Booking cancelled");
    }

    public async Task<ApiResponse<List<AvailableSlotDto>>> GetAvailableSlotsAsync(int courtId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        var bookings = await _unitOfWork.Bookings.FindAsync(b => 
            b.CourtId == courtId && 
            b.Status != BookingStatus.Cancelled &&
            b.StartTime >= startOfDay && 
            b.EndTime < endOfDay);

        // Logic đơn giản: Trả về danh sách booking đã có để FE hiển thị vùng đỏ
        var slots = bookings.Select(b => new AvailableSlotDto 
        { 
            StartTime = b.StartTime, 
            EndTime = b.EndTime, 
            IsAvailable = false 
        }).ToList();

        return ApiResponse<List<AvailableSlotDto>>.SuccessResponse(slots);
    }

    public async Task ScanAndCancelExpiredBookingsAsync()
    {
        // Tìm các booking ở trạng thái PendingPayment quá 15 phút
        var threshold = DateTime.UtcNow.AddMinutes(-15);
        var expiredBookings = await _unitOfWork.Bookings.FindAsync(b => 
            b.Status == BookingStatus.PendingPayment && 
            b.CreatedDate <= threshold);

        if (expiredBookings.Any())
        {
            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.Notes = (booking.Notes ?? "") + " [Auto-cancelled due to timeout]";
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<ApiResponse<bool>> PayBookingAsync(int bookingId, int memberId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) return ApiResponse<bool>.ErrorResponse("Booking not found");
        if (booking.MemberId != memberId) return ApiResponse<bool>.ErrorResponse("Unauthorized");
        if (booking.Status != BookingStatus.PendingPayment) return ApiResponse<bool>.ErrorResponse("Booking is not pending payment");

        var paymentResult = await _walletService.PayBookingAsync(memberId, booking.TotalPrice, booking.Id.ToString());
        if (!paymentResult.Success)
            return ApiResponse<bool>.ErrorResponse($"Payment failed: {paymentResult.Message}");

        booking.Status = BookingStatus.Confirmed;
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Payment successful");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int bookingId, int memberId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) return ApiResponse<bool>.ErrorResponse("Booking not found");
        if (booking.MemberId != memberId) return ApiResponse<bool>.ErrorResponse("Unauthorized");
        if (booking.Status == BookingStatus.Confirmed) return ApiResponse<bool>.ErrorResponse("Cannot delete confirmed booking. Please cancel it first.");

        _unitOfWork.Bookings.Remove(booking);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Booking deleted");
    }

    public async Task<ApiResponse<bool>> CheckInAsync(int bookingId, int memberId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) return ApiResponse<bool>.ErrorResponse("Booking not found");
        if (booking.MemberId != memberId) return ApiResponse<bool>.ErrorResponse("Unauthorized");
        if (booking.Status != BookingStatus.Confirmed) return ApiResponse<bool>.ErrorResponse("Booking is not confirmed");
        if (booking.IsCheckedIn) return ApiResponse<bool>.ErrorResponse("Already checked in");

        // Check if within check-in window (30 minutes before start time)
        var now = DateTime.UtcNow;
        var checkInWindow = booking.StartTime.AddMinutes(-30);
        if (now < checkInWindow) return ApiResponse<bool>.ErrorResponse("Too early to check in. Check-in opens 30 minutes before booking time.");
        if (now > booking.EndTime) return ApiResponse<bool>.ErrorResponse("Booking time has passed");

        booking.IsCheckedIn = true;
        booking.CheckInTime = now;
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Checked in successfully");
    }
}
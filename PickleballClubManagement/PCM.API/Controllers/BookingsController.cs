using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Bookings;
using PCM.Application.DTOs.Common;
using PCM.Application.Interfaces;
using System.Security.Claims;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IMemberService _memberService;

    public BookingsController(IBookingService bookingService, IMemberService memberService)
    {
        _bookingService = bookingService;
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BookingDto>>>> GetAllBookings([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _bookingService.GetAllAsync(pageNumber, pageSize);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetBookingById(int id)
    {
        var result = await _bookingService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateBooking([FromBody] BookingCreateDto dto)
    {
        try
        {
            Console.WriteLine($"[BookingsController] Received booking request: CourtId={dto.CourtId}, Start={dto.StartTime}, End={dto.EndTime}");
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"[BookingsController] UserId from token: {userId}");
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var member = await _memberService.GetByUserIdAsync(userId);
            Console.WriteLine($"[BookingsController] Member lookup result: Success={member?.Success}, Data={member?.Data?.Id}");
            
            if (member?.Data == null)
            {
                Console.WriteLine($"[BookingsController] ERROR: Member not found for UserId={userId}");
                return BadRequest(ApiResponse<BookingDto>.ErrorResponse("Không tìm thấy thông tin thành viên"));
            }

            Console.WriteLine($"[BookingsController] Calling BookingService.CreateAsync with MemberId={member.Data.Id}");
            var result = await _bookingService.CreateAsync(dto, member.Data.Id);
            Console.WriteLine($"[BookingsController] BookingService result: Success={result.Success}, Message={result.Message}");
            
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BookingsController] EXCEPTION: {ex.Message}");
            Console.WriteLine($"[BookingsController] StackTrace: {ex.StackTrace}");
            return BadRequest(ApiResponse<BookingDto>.ErrorResponse($"Lỗi hệ thống: {ex.Message}"));
        }
    }

    [HttpPost("recurring")]
    public async Task<ActionResult<ApiResponse<List<BookingDto>>>> CreateRecurringBooking([FromBody] RecurringBookingDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
            
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member?.Data == null)
            return Unauthorized(ApiResponse<List<BookingDto>>.ErrorResponse("Không tìm thấy thông tin thành viên"));

        var result = await _bookingService.CreateRecurringAsync(dto, member.Data.Id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> CancelBooking(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
            
        var member = await _memberService.GetByUserIdAsync(userId);
        if (member?.Data == null)
            return Unauthorized(ApiResponse<bool>.ErrorResponse("Không tìm thấy thông tin thành viên"));

        var result = await _bookingService.CancelAsync(id, member.Data.Id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("available-slots")]
    public async Task<ActionResult<ApiResponse<List<AvailableSlotDto>>>> GetAvailableSlots([FromQuery] int courtId, [FromQuery] DateTime date)
    {
        var result = await _bookingService.GetAvailableSlotsAsync(courtId, date);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
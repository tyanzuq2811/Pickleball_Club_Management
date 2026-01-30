using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;

namespace PCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogsController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<ActivityLogDto>>>> GetAll(
            [FromQuery] string? userName = null,
            [FromQuery] string? actionType = null,
            [FromQuery] string? entityType = null,
            [FromQuery] DateTime? date = null)
        {
            var logs = await _activityLogService.GetAllAsync(userName, actionType, entityType, date);
            return Ok(new ApiResponse<List<ActivityLogDto>>
            {
                Success = true,
                Data = logs,
                Message = "Lấy danh sách nhật ký thành công"
            });
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ActivityStatsDto>>> GetStats()
        {
            var stats = await _activityLogService.GetStatsAsync();
            return Ok(new ApiResponse<ActivityStatsDto>
            {
                Success = true,
                Data = stats,
                Message = "Lấy thống kê thành công"
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ActivityLogDto>>> GetById(int id)
        {
            var log = await _activityLogService.GetByIdAsync(id);
            if (log == null)
            {
                return NotFound(new ApiResponse<ActivityLogDto>
                {
                    Success = false,
                    Message = "Không tìm thấy nhật ký"
                });
            }

            return Ok(new ApiResponse<ActivityLogDto>
            {
                Success = true,
                Data = log,
                Message = "Lấy thông tin nhật ký thành công"
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateActivityLogDto dto)
        {
            var id = await _activityLogService.CreateAsync(dto);
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Data = id,
                Message = "Tạo nhật ký thành công"
            });
        }
    }

    public interface IActivityLogService
    {
        Task<List<ActivityLogDto>> GetAllAsync(string? userName, string? actionType, string? entityType, DateTime? date);
        Task<ActivityStatsDto> GetStatsAsync();
        Task<ActivityLogDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateActivityLogDto dto);
    }

    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string ActionType { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout, Approve, Reject, Payment
        public string EntityType { get; set; } = string.Empty; // Booking, Transaction, Member, Court, Match, Tournament, News
        public int? EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CreateActivityLogDto
    {
        public int? UserId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }

    public class ActivityStatsDto
    {
        public int TotalToday { get; set; }
        public int TotalLogins { get; set; }
        public int TotalChanges { get; set; }
        public int TotalErrors { get; set; }
    }
}

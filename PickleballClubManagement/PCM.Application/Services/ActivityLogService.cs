using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;

namespace PCM.Application.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActivityLogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(string userId, string action, string description, string? entityType = null, int? entityId = null, string? ipAddress = null)
    {
        var log = new ActivityLog
        {
            UserId = userId,
            Action = action,
            Description = description,
            EntityType = entityType,
            EntityId = entityId,
            IpAddress = ipAddress,
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.ActivityLogs.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }
}
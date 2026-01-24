namespace PCM.Application.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(string userId, string action, string description, string? entityType = null, int? entityId = null, string? ipAddress = null);
}
namespace PCM.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationToUserAsync(string userId, string message);
    Task BroadcastBookingUpdateAsync(int courtId);
}
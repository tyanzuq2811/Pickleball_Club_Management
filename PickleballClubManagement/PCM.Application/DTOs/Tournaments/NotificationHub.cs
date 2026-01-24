using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PCM.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task BroadcastBookingUpdate(int courtId)
    {
        await Clients.All.SendAsync("BookingUpdated", courtId);
    }
}
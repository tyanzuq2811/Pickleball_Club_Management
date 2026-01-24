using Microsoft.AspNetCore.SignalR;

namespace PCM.API.Hubs;

public class ScoreboardHub : Hub
{
    // Client join room theo TournamentId hoặc MatchId để nhận update riêng biệt
    public async Task JoinMatchGroup(string matchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Match_{matchId}");
    }

    public async Task LeaveMatchGroup(string matchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Match_{matchId}");
    }
}
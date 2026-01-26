using Microsoft.AspNetCore.SignalR;

namespace PCM.API.Hubs;

public class ScoreboardHub : Hub
{
    public async Task UpdateScore(int matchId, int team1Score, int team2Score)
    {
        await Clients.All.SendAsync("ReceiveScoreUpdate", matchId, team1Score, team2Score);
    }

    public async Task NotifyMatchStart(int matchId, string matchInfo)
    {
        await Clients.All.SendAsync("MatchStarted", matchId, matchInfo);
    }

    public async Task NotifyMatchEnd(int matchId, string winner)
    {
        await Clients.All.SendAsync("MatchEnded", matchId, winner);
    }
}

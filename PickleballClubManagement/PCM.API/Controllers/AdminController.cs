using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Application.DTOs.Common;
using PCM.Domain.Entities;
using PCM.Domain.Enums;
using PCM.Infrastructure.Data;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Thêm dữ liệu demo cho tournaments và participants
    /// </summary>
    [HttpPost("seed-tournament-demo")]
    public async Task<ActionResult<ApiResponse<object>>> SeedTournamentDemo()
    {
        var random = new Random();
        var admin = await _context.Members.FirstAsync();
        var allMembers = await _context.Members
            .Where(m => !m.Email.Contains("admin") && !m.Email.Contains("treasurer") && !m.Email.Contains("referee"))
            .ToListAsync();

        if (allMembers.Count < 8)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Cần ít nhất 8 hội viên để tạo demo data"));
        }

        var addedTournaments = 0;
        var addedParticipants = 0;

        // Kiểm tra và thêm tournaments mới
        var existingTournamentTitles = await _context.Tournaments.Select(t => t.Title).ToListAsync();
        var tournamentsToAdd = new List<Tournament>();

        var demoTournaments = new[]
        {
            new Tournament
            {
                Title = "Giải Mở Rộng Mùa Hè 2025",
                Type = TournamentType.Professional,
                GameMode = GameMode.Knockout,
                Status = TournamentStatus.Open,
                EntryFee = 200000,
                PrizePool = 5000000,
                CreatedBy = admin.Id,
                StartDate = DateTime.UtcNow.AddDays(21),
                EndDate = DateTime.UtcNow.AddDays(23),
                CreatedDate = DateTime.UtcNow
            },
            new Tournament
            {
                Title = "Giải Đội Hình Mạnh Nhất",
                Type = TournamentType.MiniGame,
                GameMode = GameMode.TeamBattle,
                Status = TournamentStatus.Open,
                Config_TargetWins = 7,
                CurrentScore_TeamA = 0,
                CurrentScore_TeamB = 0,
                EntryFee = 80000,
                PrizePool = 1000000,
                CreatedBy = admin.Id,
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(10),
                CreatedDate = DateTime.UtcNow
            },
            new Tournament
            {
                Title = "Giải Tài Năng Trẻ",
                Type = TournamentType.Professional,
                GameMode = GameMode.Knockout,
                Status = TournamentStatus.Ongoing,
                EntryFee = 50000,
                PrizePool = 500000,
                CreatedBy = admin.Id,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(2),
                CreatedDate = DateTime.UtcNow.AddDays(-7)
            },
            new Tournament
            {
                Title = "Giải Vô Địch Tháng Trước",
                Type = TournamentType.Professional,
                GameMode = GameMode.Knockout,
                Status = TournamentStatus.Finished,
                EntryFee = 100000,
                PrizePool = 2000000,
                CreatedBy = admin.Id,
                StartDate = DateTime.UtcNow.AddDays(-20),
                EndDate = DateTime.UtcNow.AddDays(-18),
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            }
        };

        foreach (var t in demoTournaments)
        {
            if (!existingTournamentTitles.Contains(t.Title))
            {
                tournamentsToAdd.Add(t);
            }
        }

        if (tournamentsToAdd.Any())
        {
            _context.Tournaments.AddRange(tournamentsToAdd);
            await _context.SaveChangesAsync();
            addedTournaments = tournamentsToAdd.Count;

            // Thêm participants cho tournaments mới
            foreach (var tournament in tournamentsToAdd)
            {
                var participantCount = tournament.Type == TournamentType.MiniGame 
                    ? random.Next(6, 10) 
                    : random.Next(8, 16);
                participantCount = Math.Min(participantCount, allMembers.Count);

                var selectedMembers = allMembers.OrderBy(x => random.Next()).Take(participantCount).ToList();
                
                for (int i = 0; i < selectedMembers.Count; i++)
                {
                    var participant = new Participant
                    {
                        TournamentId = tournament.Id,
                        MemberId = selectedMembers[i].Id,
                        Team = tournament.GameMode == GameMode.TeamBattle 
                            ? (i % 2 == 0 ? TeamSide.TeamA : TeamSide.TeamB) 
                            : TeamSide.None,
                        EntryFeePaid = tournament.Status != TournamentStatus.Open || random.Next(100) < 85,
                        EntryFeeAmount = tournament.EntryFee,
                        JoinedDate = tournament.CreatedDate.AddDays(random.Next(1, 5)),
                        Status = tournament.Status == TournamentStatus.Finished 
                            ? (random.Next(100) < 80 ? ParticipantStatus.Confirmed : ParticipantStatus.Eliminated)
                            : (random.Next(100) < 90 ? ParticipantStatus.Confirmed : ParticipantStatus.Pending),
                        SeedNo = i < 4 && tournament.Type == TournamentType.Professional ? i + 1 : null
                    };
                    _context.Participants.Add(participant);
                    addedParticipants++;
                }
            }
            await _context.SaveChangesAsync();
        }

        // Thêm participants cho tournaments hiện có nếu chưa có nhiều
        var existingTournaments = await _context.Tournaments
            .Where(t => t.Status == TournamentStatus.Open || t.Status == TournamentStatus.Ongoing)
            .ToListAsync();

        foreach (var tournament in existingTournaments)
        {
            var existingParticipantIds = await _context.Participants
                .Where(p => p.TournamentId == tournament.Id)
                .Select(p => p.MemberId)
                .ToListAsync();

            var currentCount = existingParticipantIds.Count;
            var targetCount = tournament.Type == TournamentType.MiniGame 
                ? random.Next(8, 12) 
                : random.Next(12, 18);

            if (currentCount < targetCount)
            {
                var availableMembers = allMembers.Where(m => !existingParticipantIds.Contains(m.Id)).ToList();
                var toAdd = Math.Min(targetCount - currentCount, availableMembers.Count);
                var newMembers = availableMembers.OrderBy(x => random.Next()).Take(toAdd).ToList();

                foreach (var member in newMembers)
                {
                    var participant = new Participant
                    {
                        TournamentId = tournament.Id,
                        MemberId = member.Id,
                        Team = tournament.GameMode == GameMode.TeamBattle 
                            ? (random.Next(2) == 0 ? TeamSide.TeamA : TeamSide.TeamB) 
                            : TeamSide.None,
                        EntryFeePaid = random.Next(100) < 80,
                        EntryFeeAmount = tournament.EntryFee,
                        JoinedDate = DateTime.UtcNow.AddDays(-random.Next(1, 5)),
                        Status = random.Next(100) < 90 ? ParticipantStatus.Confirmed : ParticipantStatus.Pending
                    };
                    _context.Participants.Add(participant);
                    addedParticipants++;
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            AddedTournaments = addedTournaments,
            AddedParticipants = addedParticipants,
            Message = $"Đã thêm {addedTournaments} giải đấu và {addedParticipants} người tham gia"
        }));
    }

    /// <summary>
    /// Lấy thống kê demo data
    /// </summary>
    [HttpGet("demo-stats")]
    public async Task<ActionResult<ApiResponse<object>>> GetDemoStats()
    {
        var stats = new
        {
            TotalMembers = await _context.Members.CountAsync(),
            TotalTournaments = await _context.Tournaments.CountAsync(),
            OpenTournaments = await _context.Tournaments.CountAsync(t => t.Status == TournamentStatus.Open),
            OngoingTournaments = await _context.Tournaments.CountAsync(t => t.Status == TournamentStatus.Ongoing),
            FinishedTournaments = await _context.Tournaments.CountAsync(t => t.Status == TournamentStatus.Finished),
            TotalParticipants = await _context.Participants.CountAsync(),
            TotalMatches = await _context.Matches.CountAsync(),
            TotalBookings = await _context.Bookings.CountAsync(),
            TotalCourts = await _context.Courts.CountAsync(),
            TotalNews = await _context.News.CountAsync(),
            TotalTransactions = await _context.Transactions.CountAsync()
        };

        return Ok(ApiResponse<object>.SuccessResponse(stats));
    }
}

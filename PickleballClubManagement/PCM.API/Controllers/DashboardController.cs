using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCM.Application.DTOs.Common;
using PCM.Domain.Interfaces;
using PCM.Domain.Enums;

namespace PCM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin,Treasurer")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats([FromQuery] int days = 30)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            var members = await _unitOfWork.Members.GetAllAsync();
            var tournaments = await _unitOfWork.Tournaments.GetAllAsync();
            var matches = await _unitOfWork.Matches.GetAllAsync();
            var courts = await _unitOfWork.Courts.GetAllAsync();

            var periodBookings = bookings.Where(b => b.CreatedDate >= startDate).ToList();
            var confirmedBookings = periodBookings.Where(b => b.Status == BookingStatus.Confirmed).ToList();

            var totalRevenue = confirmedBookings.Sum(b => b.TotalPrice);
            var previousPeriodBookings = bookings.Where(b => b.CreatedDate >= startDate.AddDays(-days) && b.CreatedDate < startDate).ToList();
            var previousRevenue = previousPeriodBookings.Where(b => b.Status == BookingStatus.Confirmed).Sum(b => b.TotalPrice);
            var revenueGrowth = previousRevenue > 0 ? (double)(((totalRevenue - previousRevenue) / previousRevenue) * 100) : 0;

            var stats = new DashboardStatsDto
            {
                TotalRevenue = totalRevenue,
                RevenueGrowth = Math.Round(revenueGrowth, 1),
                TotalBookings = periodBookings.Count,
                BookingCompletionRate = periodBookings.Count > 0 
                    ? Math.Round((double)confirmedBookings.Count / periodBookings.Count * 100, 0) 
                    : 0,
                ActiveMembers = members.Count(m => bookings.Any(b => b.MemberId == m.Id && b.CreatedDate >= startDate)),
                NewMembersThisMonth = members.Count(m => m.CreatedDate >= DateTime.UtcNow.AddMonths(-1)),
                ActiveTournaments = tournaments.Count(t => t.Status == TournamentStatus.Ongoing),
                TotalMatches = matches.Count(m => m.Date >= startDate),
                
                // Chart Data: Revenue by day
                RevenueChartLabels = Enumerable.Range(0, Math.Min(days, 30))
                    .Select(i => startDate.AddDays(i).ToString("dd/MM"))
                    .ToList(),
                RevenueChartData = Enumerable.Range(0, Math.Min(days, 30))
                    .Select(i =>
                    {
                        var dayStart = startDate.AddDays(i);
                        var dayEnd = dayStart.AddDays(1);
                        return bookings
                            .Where(b => b.CreatedDate >= dayStart && b.CreatedDate < dayEnd && b.Status == BookingStatus.Confirmed)
                            .Sum(b => (double)b.TotalPrice);
                    })
                    .ToList(),
                
                // Chart Data: Booking status distribution
                BookingStatusLabels = new List<string> { "Đã xác nhận", "Chờ thanh toán", "Đã hủy" },
                BookingStatusData = new List<int>
                {
                    periodBookings.Count(b => b.Status == BookingStatus.Confirmed),
                    periodBookings.Count(b => b.Status == BookingStatus.PendingPayment),
                    periodBookings.Count(b => b.Status == BookingStatus.Cancelled)
                },
                
                // Chart Data: Court usage percentage
                CourtUsageLabels = courts.OrderBy(c => c.Id).Take(6).Select(c => c.Name).ToList(),
                CourtUsageData = courts.OrderBy(c => c.Id).Take(6).Select(c =>
                {
                    var courtBookings = bookings.Count(b => b.CourtId == c.Id && b.CreatedDate >= startDate && b.Status == BookingStatus.Confirmed);
                    var totalSlots = days * 14; // Assuming 14 hours per day (7AM-9PM)
                    return courtBookings > 0 ? Math.Round((double)courtBookings / totalSlots * 100, 1) : 0;
                }).ToList(),
                
                // Chart Data: Member growth by month (last 6 months)
                MemberGrowthLabels = Enumerable.Range(0, 6).Reverse()
                    .Select(i => DateTime.UtcNow.AddMonths(-i).ToString("MMM"))
                    .ToList(),
                MemberGrowthData = Enumerable.Range(0, 6).Reverse()
                    .Select(i =>
                    {
                        var monthStart = DateTime.UtcNow.AddMonths(-i).AddDays(-DateTime.UtcNow.Day + 1);
                        var monthEnd = monthStart.AddMonths(1);
                        return members.Count(m => m.CreatedDate >= monthStart && m.CreatedDate < monthEnd);
                    })
                    .ToList(),
                
                TopCourts = courts.Select(c => new TopCourtDto
                {
                    Id = c.Id,
                    Rank = 0,
                    Name = c.Name,
                    Bookings = bookings.Count(b => b.CourtId == c.Id && b.CreatedDate >= startDate && b.Status == BookingStatus.Confirmed),
                    Revenue = bookings.Where(b => b.CourtId == c.Id && b.CreatedDate >= startDate && b.Status == BookingStatus.Confirmed).Sum(b => b.TotalPrice)
                })
                .OrderByDescending(c => c.Bookings)
                .Take(5)
                .Select((c, index) => { c.Rank = index + 1; return c; })
                .ToList(),
                TopMembers = members
                    .Select(m => new TopMemberDto
                    {
                        Id = m.Id,
                        Name = m.FullName,
                        Initials = string.Join("", m.FullName.Split(' ').Select(n => n.FirstOrDefault()).Take(2)),
                        Bookings = bookings.Count(b => b.MemberId == m.Id && b.CreatedDate >= startDate && b.Status == BookingStatus.Confirmed),
                        TotalSpent = bookings.Where(b => b.MemberId == m.Id && b.CreatedDate >= startDate && b.Status == BookingStatus.Confirmed).Sum(b => b.TotalPrice)
                    })
                    .OrderByDescending(m => m.Bookings)
                    .Take(5)
                    .ToList()
            };

            return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats, "Dashboard stats retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<DashboardStatsDto>.ErrorResponse($"Error: {ex.Message}"));
        }
    }
}

public class DashboardStatsDto
{
    public decimal TotalRevenue { get; set; }
    public double RevenueGrowth { get; set; }
    public int TotalBookings { get; set; }
    public double BookingCompletionRate { get; set; }
    public int ActiveMembers { get; set; }
    public int NewMembersThisMonth { get; set; }
    public int ActiveTournaments { get; set; }
    public int TotalMatches { get; set; }
    
    // Chart Data
    public List<string> RevenueChartLabels { get; set; } = new();
    public List<double> RevenueChartData { get; set; } = new();
    public List<string> BookingStatusLabels { get; set; } = new();
    public List<int> BookingStatusData { get; set; } = new();
    public List<string> CourtUsageLabels { get; set; } = new();
    public List<double> CourtUsageData { get; set; } = new();
    public List<string> MemberGrowthLabels { get; set; } = new();
    public List<int> MemberGrowthData { get; set; } = new();
    
    public List<TopCourtDto> TopCourts { get; set; } = new();
    public List<TopMemberDto> TopMembers { get; set; } = new();
}

public class TopCourtDto
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Bookings { get; set; }
    public decimal Revenue { get; set; }
}

public class TopMemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public int Bookings { get; set; }
    public decimal TotalSpent { get; set; }
}

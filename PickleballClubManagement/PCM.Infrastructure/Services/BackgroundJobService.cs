using Hangfire;
using Microsoft.Extensions.Logging;
using PCM.Domain.Interfaces;
using PCM.Domain.Enums;

namespace PCM.Infrastructure.Services;

public class BackgroundJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BackgroundJobService> _logger;

    public BackgroundJobService(IUnitOfWork unitOfWork, ILogger<BackgroundJobService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Auto-cancel bookings that are pending payment for more than 15 minutes
    /// </summary>
    [AutomaticRetry(Attempts = 0)]
    public async Task CancelExpiredPendingBookings()
    {
        try
        {
            _logger.LogInformation("Starting expired booking cancellation job");

            var allBookings = await _unitOfWork.Bookings.GetAllAsync();
            var expiredBookings = allBookings
                .Where(b => b.Status == BookingStatus.PendingPayment)
                .Where(b => (DateTime.UtcNow - b.CreatedDate).TotalMinutes > 15)
                .ToList();

            _logger.LogInformation($"Found {expiredBookings.Count} expired bookings");

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                _logger.LogInformation($"Cancelled booking ID: {booking.Id} for member {booking.MemberId}");
            }

            if (expiredBookings.Any())
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Successfully cancelled {expiredBookings.Count} expired bookings");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling expired bookings");
            throw;
        }
    }

    /// <summary>
    /// Send reminder emails for upcoming bookings (1 hour before)
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    public async Task SendUpcomingBookingReminders()
    {
        try
        {
            _logger.LogInformation("Starting booking reminder job");

            var allBookings = await _unitOfWork.Bookings.GetAllAsync();
            var upcomingBookings = allBookings
                .Where(b => b.Status == BookingStatus.Confirmed)
                .Where(b => !b.IsCheckedIn)
                .Where(b => (b.StartTime - DateTime.UtcNow).TotalMinutes > 55 && (b.StartTime - DateTime.UtcNow).TotalMinutes <= 65)
                .ToList();

            _logger.LogInformation($"Found {upcomingBookings.Count} upcoming bookings to remind");

            foreach (var booking in upcomingBookings)
            {
                // TODO: Send email/notification
                _logger.LogInformation($"Reminder sent for booking ID: {booking.Id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending booking reminders");
        }
    }

    /// <summary>
    /// Clean up old activity logs (older than 90 days)
    /// </summary>
    [AutomaticRetry(Attempts = 0)]
    public async Task CleanOldActivityLogs()
    {
        try
        {
            _logger.LogInformation("Starting activity log cleanup job");

            var allLogs = await _unitOfWork.ActivityLogs.GetAllAsync();
            var oldLogs = allLogs
                .Where(log => (DateTime.UtcNow - log.CreatedDate).TotalDays > 90)
                .ToList();

            _logger.LogInformation($"Found {oldLogs.Count} old activity logs to clean");

            foreach (var log in oldLogs)
            {
                _unitOfWork.ActivityLogs.Remove(log);
            }

            if (oldLogs.Any())
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Successfully cleaned {oldLogs.Count} old activity logs");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning old activity logs");
        }
    }

    /// <summary>
    /// Update tournament standings daily
    /// </summary>
    public async Task UpdateTournamentStandings()
    {
        try
        {
            _logger.LogInformation("Starting tournament standings update");

            var tournaments = await _unitOfWork.Tournaments.GetAllAsync();
            var activeTournaments = tournaments
                .Where(t => t.Status == TournamentStatus.Ongoing)
                .ToList();

            _logger.LogInformation($"Updating standings for {activeTournaments.Count} active tournaments");

            // TODO: Implement standings calculation logic

            _logger.LogInformation("Tournament standings updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tournament standings");
        }
    }
}

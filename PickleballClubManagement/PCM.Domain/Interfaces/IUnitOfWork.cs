using PCM.Domain.Entities;

namespace PCM.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Member> Members { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    IRepository<News> News { get; }
    IRepository<TransactionCategory> TransactionCategories { get; }
    IRepository<WalletTransaction> WalletTransactions { get; }
    IRepository<Transaction> Transactions { get; }
    IRepository<Court> Courts { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Tournament> Tournaments { get; }
    IRepository<Participant> Participants { get; }
    IRepository<TournamentMatch> TournamentMatches { get; }
    IRepository<Match> Matches { get; }
    IRepository<MatchScore> MatchScores { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<ActivityLog> ActivityLogs { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

using Microsoft.EntityFrameworkCore.Storage;
using PCM.Domain.Entities;
using PCM.Domain.Interfaces;
using PCM.Infrastructure.Data;

namespace PCM.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        
        Members = new Repository<Member>(_context);
        RefreshTokens = new Repository<RefreshToken>(_context);
        News = new Repository<News>(_context);
        TransactionCategories = new Repository<TransactionCategory>(_context);
        WalletTransactions = new Repository<WalletTransaction>(_context);
        Transactions = new Repository<Transaction>(_context);
        Courts = new Repository<Court>(_context);
        Bookings = new Repository<Booking>(_context);
        Tournaments = new Repository<Tournament>(_context);
        Participants = new Repository<Participant>(_context);
        TournamentMatches = new Repository<TournamentMatch>(_context);
        Matches = new Repository<Match>(_context);
        MatchScores = new Repository<MatchScore>(_context);
        Notifications = new Repository<Notification>(_context);
        ActivityLogs = new Repository<ActivityLog>(_context);
    }

    public IRepository<Member> Members { get; private set; }
    public IRepository<RefreshToken> RefreshTokens { get; private set; }
    public IRepository<News> News { get; private set; }
    public IRepository<TransactionCategory> TransactionCategories { get; private set; }
    public IRepository<WalletTransaction> WalletTransactions { get; private set; }
    public IRepository<Transaction> Transactions { get; private set; }
    public IRepository<Court> Courts { get; private set; }
    public IRepository<Booking> Bookings { get; private set; }
    public IRepository<Tournament> Tournaments { get; private set; }
    public IRepository<Participant> Participants { get; private set; }
    public IRepository<TournamentMatch> TournamentMatches { get; private set; }
    public IRepository<Match> Matches { get; private set; }
    public IRepository<MatchScore> MatchScores { get; private set; }
    public IRepository<Notification> Notifications { get; private set; }
    public IRepository<ActivityLog> ActivityLogs { get; private set; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

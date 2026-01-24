using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCM.Domain.Entities;

namespace PCM.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets với tên bảng theo mã sinh viên 189
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<News> News { get; set; } = null!;
    public DbSet<TransactionCategory> TransactionCategories { get; set; } = null!;
    public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Court> Courts { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<TournamentMatch> TournamentMatches { get; set; } = null!;
    public DbSet<MatchScore> MatchScores { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Cấu hình tên bảng với prefix 189_
        builder.Entity<Member>().ToTable("189_Members");
        builder.Entity<RefreshToken>().ToTable("189_RefreshTokens");
        builder.Entity<News>().ToTable("189_News");
        builder.Entity<TransactionCategory>().ToTable("189_TransactionCategories");
        builder.Entity<WalletTransaction>().ToTable("189_WalletTransactions");
        builder.Entity<Transaction>().ToTable("189_Transactions");
        builder.Entity<Court>().ToTable("189_Courts");
        builder.Entity<Booking>().ToTable("189_Bookings");
        builder.Entity<Tournament>().ToTable("189_Tournaments");
        builder.Entity<Participant>().ToTable("189_Participants");
        builder.Entity<Match>().ToTable("189_Matches");
        builder.Entity<TournamentMatch>().ToTable("189_TournamentMatches");
        builder.Entity<MatchScore>().ToTable("189_MatchScores");
        builder.Entity<Notification>().ToTable("189_Notifications");
        builder.Entity<ActivityLog>().ToTable("189_ActivityLogs");

        // Member configuration
        builder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.WalletBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Booking configuration
        builder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Court)
                .WithMany(c => c.Bookings)
                .HasForeignKey(e => e.CourtId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Member)
                .WithMany(m => m.Bookings)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.CourtId, e.StartTime, e.EndTime });
        });

        // WalletTransaction configuration
        builder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Member)
                .WithMany(m => m.WalletTransactions)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Category)
                .WithMany(c => c.WalletTransactions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Transaction configuration
        builder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Tournament configuration
        builder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntryFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PrizePool).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Participant configuration
        builder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntryFeeAmount).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Tournament)
                .WithMany(t => t.Participants)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Member)
                .WithMany(m => m.Participants)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.TournamentId, e.MemberId }).IsUnique();
        });

        // Match configuration
        builder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Tournament)
                .WithMany(t => t.Matches)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Team1_Player1)
                .WithMany()
                .HasForeignKey(e => e.Team1_Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Team1_Player2)
                .WithMany()
                .HasForeignKey(e => e.Team1_Player2Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Team2_Player1)
                .WithMany()
                .HasForeignKey(e => e.Team2_Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Team2_Player2)
                .WithMany()
                .HasForeignKey(e => e.Team2_Player2Id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TournamentMatch configuration
        builder.Entity<TournamentMatch>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Tournament)
                .WithMany(t => t.TournamentMatches)
                .HasForeignKey(e => e.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Match)
                .WithMany()
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MatchScore configuration
        builder.Entity<MatchScore>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Match)
                .WithMany(m => m.MatchScores)
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // News configuration
        builder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Notification configuration
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Member)
                .WithMany(m => m.Notifications)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        // Use fixed date for seed data to avoid migration changes
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        builder.Entity<TransactionCategory>().HasData(
            new TransactionCategory { Id = 1, Name = "Nạp tiền ví", Type = Domain.Enums.TransactionType.Income, CreatedDate = seedDate },
            new TransactionCategory { Id = 2, Name = "Phí đặt sân", Type = Domain.Enums.TransactionType.Income, CreatedDate = seedDate },
            new TransactionCategory { Id = 3, Name = "Phí tham gia giải", Type = Domain.Enums.TransactionType.Income, CreatedDate = seedDate },
            new TransactionCategory { Id = 4, Name = "Tiền thưởng giải đấu", Type = Domain.Enums.TransactionType.Expense, CreatedDate = seedDate },
            new TransactionCategory { Id = 5, Name = "Hoàn tiền hủy sân", Type = Domain.Enums.TransactionType.Expense, CreatedDate = seedDate },
            new TransactionCategory { Id = 6, Name = "Chi phí bảo trì", Type = Domain.Enums.TransactionType.Expense, CreatedDate = seedDate },
            new TransactionCategory { Id = 7, Name = "Chi phí sự kiện", Type = Domain.Enums.TransactionType.Expense, CreatedDate = seedDate }
        );

        builder.Entity<Court>().HasData(
            new Court { Id = 1, Name = "Sân 1", IsActive = true, Description = "Sân chính, có đèn chiếu sáng", CreatedDate = seedDate },
            new Court { Id = 2, Name = "Sân 2", IsActive = true, Description = "Sân phụ, thích hợp luyện tập", CreatedDate = seedDate }
        );
    }
}
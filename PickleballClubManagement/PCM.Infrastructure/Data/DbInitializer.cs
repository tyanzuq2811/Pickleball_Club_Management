using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PCM.Domain.Entities;
using PCM.Domain.Enums;

namespace PCM.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Tạo database nếu chưa có
        context.Database.Migrate();

        // Seed Roles
        string[] roles = { "Admin", "Treasurer", "Referee", "Member" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed Admin User
        var adminEmail = "admin@pcm.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            // Tạo Member profile cho Admin
            var adminMember = new Member
            {
                UserId = adminUser.Id,
                FullName = "Quản Trị Viên",
                Email = adminEmail,
                PhoneNumber = "0123456789",
                JoinDate = DateTime.UtcNow,
                RankELO = 1500,
                WalletBalance = 0,
                IsActive = true
            };
            
            context.Members.Add(adminMember);
        }

        // Seed Treasurer (Thủ quỹ)
        var treasurerEmail = "treasurer@pcm.com";
        if (await userManager.FindByEmailAsync(treasurerEmail) == null)
        {
            var user = new IdentityUser { UserName = treasurerEmail, Email = treasurerEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Treasurer@123");
            await userManager.AddToRoleAsync(user, "Treasurer");

            context.Members.Add(new Member
            {
                UserId = user.Id,
                FullName = "Thủ Quỹ CLB",
                Email = treasurerEmail,
                JoinDate = DateTime.UtcNow,
                IsActive = true,
                RankELO = 1200
            });
        }

        // Seed Referee (Trọng tài)
        var refereeEmail = "referee@pcm.com";
        if (await userManager.FindByEmailAsync(refereeEmail) == null)
        {
            var user = new IdentityUser { UserName = refereeEmail, Email = refereeEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Referee@123");
            await userManager.AddToRoleAsync(user, "Referee");

            context.Members.Add(new Member
            {
                UserId = user.Id,
                FullName = "Trọng Tài Chính",
                Email = refereeEmail,
                JoinDate = DateTime.UtcNow,
                IsActive = true,
                RankELO = 1200
            });
        }

        // Seed Sample Members
        var sampleMembers = new List<(string Name, string Email, double Rank)>
        {
            ("Hội Viên 1", "member1@pcm.com", 1200),
            ("Nguyễn Văn A", "nguyenvana@pcm.com", 1300),
            ("Trần Thị B", "tranthib@pcm.com", 1250),
            ("Lê Văn C", "levanc@pcm.com", 1400),
            ("Phạm Thị D", "phamthid@pcm.com", 1350),
            ("Hoàng Văn E", "hoangvane@pcm.com", 1200),
            ("Vũ Thị F", "vuthif@pcm.com", 1280)
        };

        var random = new Random();
        foreach (var (name, email, rank) in sampleMembers)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(user, "Member@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Member");

                    var member = new Member
                    {
                        UserId = user.Id,
                        FullName = name,
                        Email = email,
                        PhoneNumber = $"09{random.Next(10000000, 99999999)}",
                        DateOfBirth = DateTime.UtcNow.AddYears(-random.Next(20, 40)),
                        JoinDate = DateTime.UtcNow.AddMonths(-random.Next(1, 12)),
                        RankELO = rank,
                        WalletBalance = 500000,
                        IsActive = true
                    };
                    
                    context.Members.Add(member);
                }
            }
        }

        await context.SaveChangesAsync();

        // Seed Courts
        if (!await context.Courts.AnyAsync())
        {
            var courts = new List<Court>
            {
                new Court { Name = "Sân 1", Description = "Sân chính - Có đèn", IsActive = true, PricePerHour = 100000 },
                new Court { Name = "Sân 2", Description = "Sân phụ - Có mái che", IsActive = true, PricePerHour = 80000 },
                new Court { Name = "Sân 3", Description = "Sân VIP - Điều hòa", IsActive = true, PricePerHour = 150000 }
            };
            context.Courts.AddRange(courts);
            await context.SaveChangesAsync();
        }

        // Seed Transaction Categories
        if (!await context.TransactionCategories.AnyAsync())
        {
            var categories = new List<TransactionCategory>
            {
                new TransactionCategory { Name = "Nạp tiền", Type = TransactionType.Income },
                new TransactionCategory { Name = "Phí sân", Type = TransactionType.Expense },
                new TransactionCategory { Name = "Thưởng giải", Type = TransactionType.Expense },
                new TransactionCategory { Name = "Hoàn tiền", Type = TransactionType.Income },
                new TransactionCategory { Name = "Đóng góp", Type = TransactionType.Income },
                new TransactionCategory { Name = "Sửa chữa", Type = TransactionType.Expense },
                new TransactionCategory { Name = "Tiện ích", Type = TransactionType.Expense }
            };
            context.TransactionCategories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Initial Club Fund
        if (!await context.Transactions.AnyAsync())
        {
            var admin = await context.Members.FirstAsync();
            var incomeCategory = await context.TransactionCategories.FirstAsync(c => c.Name == "Đóng góp");
            
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Date = DateTime.UtcNow.AddMonths(-2),
                    Amount = 5000000,
                    Description = "Quỹ khởi tạo CLB",
                    CategoryId = incomeCategory.Id,
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow.AddMonths(-2)
                },
                new Transaction
                {
                    Date = DateTime.UtcNow.AddMonths(-1),
                    Amount = 2000000,
                    Description = "Quyên góp từ các hội viên",
                    CategoryId = incomeCategory.Id,
                    CreatedBy = admin.Id,
                    CreatedDate = DateTime.UtcNow.AddMonths(-1)
                }
            };
            
            context.Transactions.AddRange(transactions);
            await context.SaveChangesAsync();
        }

        // Seed Sample Bookings
        if (!await context.Bookings.AnyAsync())
        {
            var members = await context.Members.Where(m => m.Email.Contains("member") || m.Email.Contains("nguyen")).Take(3).ToListAsync();
            var courts = await context.Courts.ToListAsync();
            
            if (members.Count >= 2 && courts.Any())
            {
                var bookings = new List<Booking>
                {
                    new Booking
                    {
                        CourtId = courts[0].Id,
                        MemberId = members[0].Id,
                        StartTime = DateTime.Today.AddHours(18),
                        EndTime = DateTime.Today.AddHours(19),
                        TotalPrice = courts[0].PricePerHour,
                        Status = BookingStatus.Confirmed,
                        CreatedDate = DateTime.UtcNow
                    },
                    new Booking
                    {
                        CourtId = courts.Count > 1 ? courts[1].Id : courts[0].Id,
                        MemberId = members[1].Id,
                        StartTime = DateTime.Today.AddDays(1).AddHours(17),
                        EndTime = DateTime.Today.AddDays(1).AddHours(18),
                        TotalPrice = courts.Count > 1 ? courts[1].PricePerHour : courts[0].PricePerHour,
                        Status = BookingStatus.Confirmed,
                        CreatedDate = DateTime.UtcNow
                    }
                };
                
                context.Bookings.AddRange(bookings);
                await context.SaveChangesAsync();
            }
        }

        // Seed Sample Tournament
        if (!await context.Tournaments.AnyAsync())
        {
            var admin = await context.Members.FirstAsync();
            var tournaments = new List<Tournament>
            {
                new Tournament
                {
                    Title = "Giải Giao Hữu Tháng 1",
                    Type = TournamentType.MiniGame,
                    GameMode = GameMode.TeamBattle,
                    Status = TournamentStatus.Open,
                    Config_TargetWins = 5,
                    CurrentScore_TeamA = 0,
                    CurrentScore_TeamB = 0,
                    EntryFee = 50000,
                    PrizePool = 0,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(7),
                    CreatedDate = DateTime.UtcNow
                },
                new Tournament
                {
                    Title = "Giải Knockout Mùa Xuân",
                    Type = TournamentType.Professional,
                    GameMode = GameMode.Knockout,
                    Status = TournamentStatus.Open,
                    EntryFee = 100000,
                    PrizePool = 0,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(14),
                    CreatedDate = DateTime.UtcNow
                },
                new Tournament
                {
                    Title = "Kèo Thách Đấu Tuần 1",
                    Type = TournamentType.Duel,
                    GameMode = GameMode.None,
                    Status = TournamentStatus.Ongoing,
                    EntryFee = 0,
                    PrizePool = 0,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    CreatedDate = DateTime.UtcNow.AddDays(-3)
                }
            };
            
            context.Tournaments.AddRange(tournaments);
            await context.SaveChangesAsync();

            // Add participants to first tournament
            var members = await context.Members.Where(m => m.Email.Contains("member") || m.Email.Contains("nguyen") || m.Email.Contains("tran")).Take(6).ToListAsync();
            if (members.Count >= 4)
            {
                var firstTournament = tournaments[0];
                var participants = new List<Participant>();
                
                for (int i = 0; i < Math.Min(members.Count, 6); i++)
                {
                    participants.Add(new Participant
                    {
                        TournamentId = firstTournament.Id,
                        MemberId = members[i].Id,
                        Team = i % 2 == 0 ? TeamSide.TeamA : TeamSide.TeamB,
                        EntryFeePaid = true,
                        EntryFeeAmount = 50000,
                        JoinedDate = DateTime.UtcNow,
                        Status = ParticipantStatus.Confirmed
                    });
                }
                
                context.Participants.AddRange(participants);
                await context.SaveChangesAsync();
            }
        }

        // Seed Sample Matches
        if (!await context.Matches.AnyAsync())
        {
            var members = await context.Members.Where(m => m.Email.Contains("member") || m.Email.Contains("nguyen") || m.Email.Contains("tran") || m.Email.Contains("le")).Take(4).ToListAsync();
            if (members.Count >= 4)
            {
                var matches = new List<Match>
                {
                    new Match
                    {
                        Date = DateTime.Today.AddDays(-1),
                        IsRanked = true,
                        MatchFormat = MatchFormat.Doubles,
                        Team1_Player1Id = members[0].Id,
                        Team1_Player2Id = members[1].Id,
                        Team2_Player1Id = members[2].Id,
                        Team2_Player2Id = members[3].Id,
                        WinningSide = WinningSide.Team1,
                        EloChange = 12.5
                    },
                    new Match
                    {
                        Date = DateTime.Today.AddDays(-2),
                        IsRanked = true,
                        MatchFormat = MatchFormat.Singles,
                        Team1_Player1Id = members[0].Id,
                        Team2_Player1Id = members[1].Id,
                        WinningSide = WinningSide.Team2,
                        EloChange = 10.0
                    }
                };
                
                context.Matches.AddRange(matches);
                await context.SaveChangesAsync();

                // Add match scores
                var firstMatch = matches[0];
                var scores = new List<MatchScore>
                {
                    new MatchScore { MatchId = firstMatch.Id, SetNumber = 1, Team1Score = 11, Team2Score = 7, IsFinalSet = false },
                    new MatchScore { MatchId = firstMatch.Id, SetNumber = 2, Team1Score = 11, Team2Score = 9, IsFinalSet = true }
                };
                
                context.MatchScores.AddRange(scores);
                await context.SaveChangesAsync();
            }
        }

        // Seed Sample News
        if (!await context.News.AnyAsync())
        {
            var admin = await context.Members.FirstAsync();
            var newsList = new List<News>
            {
                new News
                {
                    Title = "Thông báo lịch nghỉ Tết Nguyên Đán 2026",
                    Summary = "CLB sẽ nghỉ hoạt động từ 28 Tết đến Mùng 5 Tết",
                    Content = "Kính gửi toàn thể hội viên,\n\nCLB Vợt Thủ Phố Núi xin thông báo lịch nghỉ Tết Nguyên Đán 2026:\n- Nghỉ từ: 28 Tết (25/01/2026)\n- Hoạt động trở lại: Mùng 6 Tết (02/02/2026)\n\nChúc các bạn một năm mới an khang thịnh vượng!",
                    IsPinned = true,
                    CreatedBy = admin.FullName,
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                new News
                {
                    Title = "Khai trương Sân 3 VIP",
                    Summary = "CLB vừa đưa vào hoạt động sân VIP với đầy đủ tiện nghi",
                    Content = "Chúng tôi vui mừng thông báo Sân 3 VIP đã chính thức hoạt động!\n\nĐặc điểm:\n- Có điều hòa\n- Ánh sáng LED chuyên dụng\n- Khán đài rộng rãi\n- Giá: 150.000đ/giờ\n\nHãy đặt sân ngay để trải nghiệm!",
                    IsPinned = false,
                    CreatedBy = admin.FullName,
                    CreatedDate = DateTime.UtcNow.AddDays(-3)
                },
                new News
                {
                    Title = "Chúc mừng top 3 giải tháng 12",
                    Summary = "Vinh danh 3 VĐV xuất sắc nhất tháng 12/2025",
                    Content = "CLB xin chúc mừng:\n\n🥇 Vị trí 1: Nguyễn Văn A - 1400 ELO\n🥈 Vị trí 2: Lê Văn C - 1380 ELO\n🥉 Vị trí 3: Phạm Thị D - 1350 ELO\n\nChúc mừng các bạn! Hẹn gặp lại trong giải tháng 1!",
                    IsPinned = false,
                    CreatedBy = admin.FullName,
                    CreatedDate = DateTime.UtcNow.AddDays(-7)
                }
            };
            
            context.News.AddRange(newsList);
            await context.SaveChangesAsync();
        }

        // Seed Sample Wallet Transactions
        var walletTransactions = new List<WalletTransaction>();
        var depositCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Nạp tiền");
        
        if (depositCategory != null)
        {
            var members = await context.Members.Where(m => m.Email.Contains("member")).Take(3).ToListAsync();
            foreach (var member in members)
            {
                walletTransactions.Add(new WalletTransaction
                {
                    Date = DateTime.UtcNow.AddDays(-10),
                    Amount = 500000,
                    MemberId = member.Id,
                    CategoryId = depositCategory.Id,
                    Type = WalletTransactionType.Deposit,
                    Description = "Nạp tiền vào ví",
                    Status = TransactionStatus.Success
                });
            }
            
            if (walletTransactions.Any())
            {
                context.WalletTransactions.AddRange(walletTransactions);
                await context.SaveChangesAsync();
            }
        }
    }
}
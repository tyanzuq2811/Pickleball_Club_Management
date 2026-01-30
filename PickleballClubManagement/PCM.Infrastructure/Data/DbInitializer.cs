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

        // Seed Sample Members - NHIỀU HƠN
        var sampleMembers = new List<(string Name, string Email, double Rank)>
        {
            ("Hội Viên 1", "member1@pcm.com", 1200),
            ("Nguyễn Văn A", "nguyenvana@pcm.com", 1300),
            ("Trần Thị B", "tranthib@pcm.com", 1250),
            ("Lê Văn C", "levanc@pcm.com", 1400),
            ("Phạm Thị D", "phamthid@pcm.com", 1350),
            ("Hoàng Văn E", "hoangvane@pcm.com", 1200),
            ("Vũ Thị F", "vuthif@pcm.com", 1280),
            ("Đặng Minh G", "dangminhg@pcm.com", 1320),
            ("Bùi Thị H", "buithih@pcm.com", 1260),
            ("Dương Văn I", "duongvani@pcm.com", 1380),
            ("Võ Thị K", "vothik@pcm.com", 1290),
            ("Phan Văn L", "phanvanl@pcm.com", 1340),
            ("Trịnh Thị M", "trinhthim@pcm.com", 1310),
            ("Lý Văn N", "lyvann@pcm.com", 1270),
            ("Mai Thị O", "maithio@pcm.com", 1360),
            ("Ngô Văn P", "ngovanp@pcm.com", 1330),
            ("Cao Thị Q", "caothiq@pcm.com", 1240),
            ("Tô Văn R", "tovanr@pcm.com", 1370),
            ("Hồ Thị S", "hothis@pcm.com", 1300),
            ("Đinh Văn T", "dinhvant@pcm.com", 1280)
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

        // Seed Courts - THÊM SÂN
        if (!await context.Courts.AnyAsync())
        {
            var courts = new List<Court>
            {
                new Court { Name = "Sân 1", Description = "Sân chính - Có đèn", IsActive = true, PricePerHour = 100000 },
                new Court { Name = "Sân 2", Description = "Sân phụ - Có mái che", IsActive = true, PricePerHour = 80000 },
                new Court { Name = "Sân 3", Description = "Sân VIP - Điều hòa", IsActive = true, PricePerHour = 150000 },
                new Court { Name = "Sân 4", Description = "Sân ngoài trời - View đẹp", IsActive = true, PricePerHour = 90000 },
                new Court { Name = "Sân 5", Description = "Sân mini - Phù hợp tập luyện", IsActive = true, PricePerHour = 70000 },
                new Court { Name = "Sân 6", Description = "Sân thi đấu - Khán đài lớn", IsActive = true, PricePerHour = 120000 }
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

        // Seed Initial Club Fund - NHIỀU GIAO DỊCH HƠN
        if (!await context.Transactions.AnyAsync())
        {
            var admin = await context.Members.FirstAsync();
            var incomeCategory = await context.TransactionCategories.FirstAsync(c => c.Name == "Đóng góp");
            var expenseCategory = await context.TransactionCategories.FirstAsync(c => c.Name == "Sửa chữa");
            var courtFeeCategory = await context.TransactionCategories.FirstAsync(c => c.Name == "Phí sân");
            
            var transactions = new List<Transaction>();
            
            // Quỹ khởi tạo
            transactions.Add(new Transaction
            {
                Date = DateTime.UtcNow.AddMonths(-3),
                Amount = 10000000,
                Description = "Quỹ khởi tạo CLB",
                CategoryId = incomeCategory.Id,
                CreatedBy = admin.Id,
                CreatedDate = DateTime.UtcNow.AddMonths(-3)
            });
            
            // Các giao dịch trong 90 ngày qua
            for (int day = -90; day < 0; day++)
            {
                var date = DateTime.UtcNow.AddDays(day);
                
                // Thu nhập từ booking (2-4 giao dịch/ngày)
                var bookingIncomes = random.Next(2, 5);
                for (int i = 0; i < bookingIncomes; i++)
                {
                    transactions.Add(new Transaction
                    {
                        Date = date,
                        Amount = random.Next(80000, 300000),
                        Description = $"Thu phí đặt sân ngày {date:dd/MM}",
                        CategoryId = courtFeeCategory.Id,
                        CreatedBy = admin.Id,
                        CreatedDate = date
                    });
                }
                
                // Chi phí (1 giao dịch mỗi 7-10 ngày)
                if (day % random.Next(7, 11) == 0)
                {
                    transactions.Add(new Transaction
                    {
                        Date = date,
                        Amount = -random.Next(100000, 500000),
                        Description = $"Chi phí {(random.Next(2) == 0 ? "sửa chữa thiết bị" : "điện nước")}",
                        CategoryId = expenseCategory.Id,
                        CreatedBy = admin.Id,
                        CreatedDate = date
                    });
                }
                
                // Đóng góp (1 giao dịch mỗi 15 ngày)
                if (day % 15 == 0)
                {
                    transactions.Add(new Transaction
                    {
                        Date = date,
                        Amount = random.Next(500000, 2000000),
                        Description = "Quyên góp từ hội viên",
                        CategoryId = incomeCategory.Id,
                        CreatedBy = admin.Id,
                        CreatedDate = date
                    });
                }
            }
            
            context.Transactions.AddRange(transactions);
            await context.SaveChangesAsync();
        }

        // Seed Sample Bookings - NHIỀU HƠN
        if (!await context.Bookings.AnyAsync())
        {
            var members = await context.Members.Where(m => !m.Email.Contains("admin") && !m.Email.Contains("treasurer") && !m.Email.Contains("referee")).ToListAsync();
            var courts = await context.Courts.ToListAsync();
            
            if (members.Count >= 5 && courts.Any())
            {
                var bookings = new List<Booking>();
                
                // Tạo bookings trong 30 ngày qua
                for (int day = -30; day <= 7; day++)
                {
                    var date = DateTime.Today.AddDays(day);
                    
                    // Mỗi ngày có 3-6 bookings random
                    var bookingsPerDay = random.Next(3, 7);
                    
                    for (int i = 0; i < bookingsPerDay; i++)
                    {
                        var court = courts[random.Next(courts.Count)];
                        var member = members[random.Next(members.Count)];
                        var startHour = random.Next(6, 21); // 6AM - 9PM
                        var duration = random.Next(1, 3); // 1-2 giờ
                        
                        BookingStatus status;
                        if (day < 0) // Past bookings
                        {
                            // 80% confirmed, 15% completed, 5% cancelled
                            var statusRoll = random.Next(100);
                            status = statusRoll < 80 ? BookingStatus.Confirmed : 
                                    statusRoll < 95 ? BookingStatus.Confirmed : BookingStatus.Cancelled;
                        }
                        else if (day == 0) // Today
                        {
                            status = startHour < DateTime.Now.Hour ? BookingStatus.Confirmed : BookingStatus.Confirmed;
                        }
                        else // Future bookings
                        {
                            // 70% confirmed, 30% pending
                            status = random.Next(100) < 70 ? BookingStatus.Confirmed : BookingStatus.PendingPayment;
                        }
                        
                        bookings.Add(new Booking
                        {
                            CourtId = court.Id,
                            MemberId = member.Id,
                            StartTime = date.AddHours(startHour),
                            EndTime = date.AddHours(startHour + duration),
                            TotalPrice = court.PricePerHour * duration,
                            Status = status,
                            CreatedDate = date.AddHours(startHour - random.Next(1, 48)), // Đặt trước 1-48 giờ
                            CheckInTime = day < 0 && status == BookingStatus.Confirmed ? (DateTime?)date.AddHours(startHour).AddMinutes(-random.Next(5, 30)) : null,
                            IsCheckedIn = day < 0 && status == BookingStatus.Confirmed
                        });
                    }
                }
                
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

        // Seed Sample Matches - NHIỀU TRẬN ĐẤU HƠN
        if (!await context.Matches.AnyAsync())
        {
            var members = await context.Members.Where(m => !m.Email.Contains("admin") && !m.Email.Contains("treasurer")).ToListAsync();
            if (members.Count >= 8)
            {
                var matches = new List<Match>();
                var matchScores = new List<MatchScore>();
                
                // Tạo 50 trận đấu trong 60 ngày qua
                for (int i = 0; i < 50; i++)
                {
                    var daysAgo = random.Next(1, 61);
                    var matchDate = DateTime.Today.AddDays(-daysAgo);
                    
                    // Random format: 60% doubles, 40% singles
                    var format = random.Next(100) < 60 ? MatchFormat.Doubles : MatchFormat.Singles;
                    
                    Match match;
                    if (format == MatchFormat.Doubles)
                    {
                        // Pick 4 random players
                        var players = members.OrderBy(x => random.Next()).Take(4).ToList();
                        match = new Match
                        {
                            Date = matchDate,
                            IsRanked = random.Next(100) < 80, // 80% ranked
                            MatchFormat = MatchFormat.Doubles,
                            Team1_Player1Id = players[0].Id,
                            Team1_Player2Id = players[1].Id,
                            Team2_Player1Id = players[2].Id,
                            Team2_Player2Id = players[3].Id,
                            WinningSide = random.Next(2) == 0 ? WinningSide.Team1 : WinningSide.Team2,
                            EloChange = random.Next(8, 16) + random.NextDouble()
                        };
                    }
                    else
                    {
                        // Pick 2 random players
                        var players = members.OrderBy(x => random.Next()).Take(2).ToList();
                        match = new Match
                        {
                            Date = matchDate,
                            IsRanked = random.Next(100) < 80,
                            MatchFormat = MatchFormat.Singles,
                            Team1_Player1Id = players[0].Id,
                            Team2_Player1Id = players[1].Id,
                            WinningSide = random.Next(2) == 0 ? WinningSide.Team1 : WinningSide.Team2,
                            EloChange = random.Next(8, 16) + random.NextDouble()
                        };
                    }
                    
                    matches.Add(match);
                }
                
                context.Matches.AddRange(matches);
                await context.SaveChangesAsync();

                // Add match scores for all matches
                foreach (var match in matches)
                {
                    var numSets = random.Next(100) < 70 ? 2 : 3; // 70% best of 2, 30% best of 3
                    
                    for (int setNum = 1; setNum <= numSets; setNum++)
                    {
                        int team1Score, team2Score;
                        
                        if (setNum == numSets) // Final set - winner wins this set
                        {
                            if (match.WinningSide == WinningSide.Team1)
                            {
                                team1Score = 11;
                                team2Score = random.Next(5, 10);
                            }
                            else
                            {
                                team1Score = random.Next(5, 10);
                                team2Score = 11;
                            }
                        }
                        else // Previous sets - random winner
                        {
                            if (random.Next(2) == 0)
                            {
                                team1Score = 11;
                                team2Score = random.Next(5, 10);
                            }
                            else
                            {
                                team1Score = random.Next(5, 10);
                                team2Score = 11;
                            }
                        }
                        
                        matchScores.Add(new MatchScore
                        {
                            MatchId = match.Id,
                            SetNumber = setNum,
                            Team1Score = team1Score,
                            Team2Score = team2Score,
                            IsFinalSet = setNum == numSets
                        });
                    }
                }
                
                context.MatchScores.AddRange(matchScores);
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

        // Seed Sample Wallet Transactions - NHIỀU HƠN
        var walletTransactions = new List<WalletTransaction>();
        var depositCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Nạp tiền");
        var bookingCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Phí sân");
        
        if (depositCategory != null && bookingCategory != null)
        {
            var members = await context.Members.Where(m => !m.Email.Contains("admin") && !m.Email.Contains("treasurer") && !m.Email.Contains("referee")).ToListAsync();
            
            // Mỗi member có 3-8 giao dịch ví
            foreach (var member in members)
            {
                var numTransactions = random.Next(3, 9);
                
                for (int i = 0; i < numTransactions; i++)
                {
                    var daysAgo = random.Next(1, 90);
                    var date = DateTime.UtcNow.AddDays(-daysAgo);
                    
                    // 60% deposit, 40% payment for booking
                    if (random.Next(100) < 60)
                    {
                        // Deposit
                        walletTransactions.Add(new WalletTransaction
                        {
                            Date = date,
                            Amount = random.Next(3, 11) * 100000, // 300k - 1M
                            MemberId = member.Id,
                            CategoryId = depositCategory.Id,
                            Type = WalletTransactionType.Deposit,
                            Description = "Nạp tiền vào ví",
                            Status = random.Next(100) < 90 ? TransactionStatus.Success : TransactionStatus.Pending // 90% success
                        });
                    }
                    else
                    {
                        // Payment for booking
                        walletTransactions.Add(new WalletTransaction
                        {
                            Date = date,
                            Amount = -random.Next(70000, 200000),
                            MemberId = member.Id,
                            CategoryId = bookingCategory.Id,
                            Type = WalletTransactionType.PayBooking,
                            Description = "Thanh toán đặt sân",
                            Status = TransactionStatus.Success
                        });
                    }
                }
            }
            
            if (walletTransactions.Any())
            {
                context.WalletTransactions.AddRange(walletTransactions);
                await context.SaveChangesAsync();
            }
        }
    }
}
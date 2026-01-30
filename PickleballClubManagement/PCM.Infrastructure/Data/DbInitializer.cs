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
                FullName = "Trần Minh Quân",
                Email = adminEmail,
                PhoneNumber = "0987654321",
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
                FullName = "Nguyễn Thị Hồng Nhung",
                Email = treasurerEmail,
                PhoneNumber = "0912345678",
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
                FullName = "Phạm Văn Hùng",
                Email = refereeEmail,
                PhoneNumber = "0923456789",
                JoinDate = DateTime.UtcNow,
                IsActive = true,
                RankELO = 1200
            });
        }

        // Seed Sample Members - NHIỀU HƠN
        var sampleMembers = new List<(string Name, string Email, double Rank)>
        {
            ("Lê Tuấn Dũng", "letuandung@pcm.com", 1200),
            ("Nguyễn Hoàng Nam", "nguyenhoangnam@pcm.com", 1300),
            ("Trần Thị Thanh Hà", "tranthanhha@pcm.com", 1250),
            ("Lê Minh Khôi", "leminhkhoi@pcm.com", 1400),
            ("Phạm Thị Ngọc Ánh", "phamngocaoh@pcm.com", 1350),
            ("Hoàng Đức Anh", "hoangducanh@pcm.com", 1200),
            ("Vũ Thị Mai Linh", "vumailink@pcm.com", 1280),
            ("Đặng Quốc Việt", "dangquocviet@pcm.com", 1320),
            ("Bùi Thị Thúy Hằng", "buithuyhang@pcm.com", 1260),
            ("Dương Trọng Nghĩa", "duongtrongnghia@pcm.com", 1380),
            ("Võ Thị Kim Chi", "vokimchi@pcm.com", 1290),
            ("Phan Thanh Tùng", "phanthanhtung@pcm.com", 1340),
            ("Trịnh Thị Như Quỳnh", "trinhnhuquynh@pcm.com", 1310),
            ("Lý Hoàng Bảo", "lyhoangbao@pcm.com", 1270),
            ("Mai Thị Bích Ngọc", "maibichngoc@pcm.com", 1360),
            ("Ngô Văn Thành", "ngovanthanh@pcm.com", 1330),
            ("Cao Thị Diệu My", "caodieumi@pcm.com", 1240),
            ("Tô Quang Huy", "toquanghuy@pcm.com", 1370),
            ("Hồ Thị Phương Vy", "hophuongvy@pcm.com", 1300),
            ("Đinh Công Minh", "dinhcongminh@pcm.com", 1280)
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
            // Sử dụng FirstOrDefaultAsync để tránh lỗi nếu category không tồn tại
            var incomeCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Nạp tiền ví") 
                                ?? await context.TransactionCategories.FirstAsync(c => c.Type == TransactionType.Income);
            var expenseCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Chi phí bảo trì")
                                ?? await context.TransactionCategories.FirstAsync(c => c.Type == TransactionType.Expense);
            var courtFeeCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Phí đặt sân")
                                ?? await context.TransactionCategories.FirstAsync(c => c.Name.Contains("sân"));
            
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
                    PrizePool = 500000,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(7),
                    EndDate = DateTime.UtcNow.AddDays(8),
                    CreatedDate = DateTime.UtcNow.AddDays(-7)
                },
                new Tournament
                {
                    Title = "Giải Knockout Mùa Xuân 2025",
                    Type = TournamentType.Professional,
                    GameMode = GameMode.Knockout,
                    Status = TournamentStatus.Open,
                    EntryFee = 100000,
                    PrizePool = 2000000,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(14),
                    EndDate = DateTime.UtcNow.AddDays(16),
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                new Tournament
                {
                    Title = "Giải Vô Địch CLB Tháng 12",
                    Type = TournamentType.Professional,
                    GameMode = GameMode.Knockout,
                    Status = TournamentStatus.Ongoing,
                    EntryFee = 150000,
                    PrizePool = 3000000,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(-3),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    CreatedDate = DateTime.UtcNow.AddDays(-14)
                },
                new Tournament
                {
                    Title = "Giải Giao Hữu Cuối Tuần",
                    Type = TournamentType.MiniGame,
                    GameMode = GameMode.TeamBattle,
                    Status = TournamentStatus.Finished,
                    Config_TargetWins = 3,
                    CurrentScore_TeamA = 3,
                    CurrentScore_TeamB = 1,
                    EntryFee = 30000,
                    PrizePool = 200000,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(-10),
                    CreatedDate = DateTime.UtcNow.AddDays(-17)
                },
                new Tournament
                {
                    Title = "Kèo Thách Đấu VIP",
                    Type = TournamentType.Duel,
                    GameMode = GameMode.None,
                    Status = TournamentStatus.Ongoing,
                    EntryFee = 0,
                    PrizePool = 0,
                    CreatedBy = admin.Id,
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    EndDate = DateTime.UtcNow.AddDays(5),
                    CreatedDate = DateTime.UtcNow.AddDays(-3)
                }
            };
            
            context.Tournaments.AddRange(tournaments);
            await context.SaveChangesAsync();

            // Add participants to tournaments - NHIỀU HƠN
            var allMembers = await context.Members.Where(m => !m.Email.Contains("admin") && !m.Email.Contains("treasurer") && !m.Email.Contains("referee")).ToListAsync();
            
            if (allMembers.Count >= 8)
            {
                var participants = new List<Participant>();
                
                // Tournament 1: Giải Giao Hữu Tháng 1 - 8 người
                var t1Members = allMembers.OrderBy(x => random.Next()).Take(8).ToList();
                for (int i = 0; i < t1Members.Count; i++)
                {
                    participants.Add(new Participant
                    {
                        TournamentId = tournaments[0].Id,
                        MemberId = t1Members[i].Id,
                        Team = i % 2 == 0 ? TeamSide.TeamA : TeamSide.TeamB,
                        EntryFeePaid = true,
                        EntryFeeAmount = 50000,
                        JoinedDate = DateTime.UtcNow.AddDays(-random.Next(1, 6)),
                        Status = ParticipantStatus.Confirmed
                    });
                }
                
                // Tournament 2: Giải Knockout Mùa Xuân - 12 người
                var t2Members = allMembers.OrderBy(x => random.Next()).Take(12).ToList();
                for (int i = 0; i < t2Members.Count; i++)
                {
                    participants.Add(new Participant
                    {
                        TournamentId = tournaments[1].Id,
                        MemberId = t2Members[i].Id,
                        Team = TeamSide.None,
                        EntryFeePaid = random.Next(100) < 80, // 80% đã thanh toán
                        EntryFeeAmount = 100000,
                        JoinedDate = DateTime.UtcNow.AddDays(-random.Next(1, 4)),
                        Status = random.Next(100) < 90 ? ParticipantStatus.Confirmed : ParticipantStatus.Pending,
                        SeedNo = i < 4 ? i + 1 : null // Top 4 được seed
                    });
                }
                
                // Tournament 3: Giải Vô Địch CLB - 16 người (đang diễn ra)
                var t3Members = allMembers.OrderBy(x => random.Next()).Take(Math.Min(16, allMembers.Count)).ToList();
                for (int i = 0; i < t3Members.Count; i++)
                {
                    participants.Add(new Participant
                    {
                        TournamentId = tournaments[2].Id,
                        MemberId = t3Members[i].Id,
                        Team = TeamSide.None,
                        EntryFeePaid = true,
                        EntryFeeAmount = 150000,
                        JoinedDate = DateTime.UtcNow.AddDays(-random.Next(7, 14)),
                        Status = ParticipantStatus.Confirmed,
                        SeedNo = i < 4 ? i + 1 : null
                    });
                }
                
                // Tournament 4: Giải Giao Hữu Cuối Tuần (đã kết thúc) - 6 người
                var t4Members = allMembers.OrderBy(x => random.Next()).Take(6).ToList();
                for (int i = 0; i < t4Members.Count; i++)
                {
                    participants.Add(new Participant
                    {
                        TournamentId = tournaments[3].Id,
                        MemberId = t4Members[i].Id,
                        Team = i % 2 == 0 ? TeamSide.TeamA : TeamSide.TeamB,
                        EntryFeePaid = true,
                        EntryFeeAmount = 30000,
                        JoinedDate = DateTime.UtcNow.AddDays(-random.Next(14, 17)),
                        Status = ParticipantStatus.Confirmed
                    });
                }
                
                // Tournament 5: Kèo Thách Đấu - 4 người
                var t5Members = allMembers.OrderBy(x => random.Next()).Take(4).ToList();
                for (int i = 0; i < t5Members.Count; i++)
                {
                    participants.Add(new Participant
                    {
                        TournamentId = tournaments[4].Id,
                        MemberId = t5Members[i].Id,
                        Team = TeamSide.None,
                        EntryFeePaid = true,
                        EntryFeeAmount = 0,
                        JoinedDate = DateTime.UtcNow.AddDays(-random.Next(1, 3)),
                        Status = ParticipantStatus.Confirmed
                    });
                }
                
                context.Participants.AddRange(participants);
                await context.SaveChangesAsync();

                // Tự động tạo bracket cho Tournament 3 (Giải Vô Địch CLB - đang diễn ra)
                var t3ParticipantIds = participants.Where(p => p.TournamentId == tournaments[2].Id).Select(p => p.MemberId).ToList();
                if (t3ParticipantIds.Count >= 4)
                {
                    // Shuffle participants
                    var shuffledIds = t3ParticipantIds.OrderBy(x => random.Next()).ToList();
                    var tournamentMatches = new List<TournamentMatch>();
                    var bracketMatches = new List<Match>();

                    // Round 1 - 8 trận (16 người)
                    int matchesInRound1 = shuffledIds.Count / 2;
                    for (int i = 0; i < matchesInRound1; i++)
                    {
                        var match = new Match
                        {
                            Date = tournaments[2].StartDate ?? DateTime.UtcNow,
                            IsRanked = true,
                            TournamentId = tournaments[2].Id,
                            MatchFormat = MatchFormat.Singles,
                            Team1_Player1Id = shuffledIds[i * 2],
                            Team2_Player1Id = shuffledIds[i * 2 + 1],
                            WinningSide = WinningSide.None,
                            CreatedDate = DateTime.UtcNow
                        };
                        bracketMatches.Add(match);
                    }

                    context.Matches.AddRange(bracketMatches);
                    await context.SaveChangesAsync();

                    // Tạo TournamentMatch cho Round 1
                    for (int i = 0; i < bracketMatches.Count; i++)
                    {
                        tournamentMatches.Add(new TournamentMatch
                        {
                            TournamentId = tournaments[2].Id,
                            MatchId = bracketMatches[i].Id,
                            Round = 1,
                            BracketGroup = "WinnerBracket"
                        });
                    }

                    // Round 2 - 4 trận
                    var round2Matches = new List<Match>();
                    for (int i = 0; i < matchesInRound1 / 2; i++)
                    {
                        var match = new Match
                        {
                            Date = (tournaments[2].StartDate ?? DateTime.UtcNow).AddDays(1),
                            IsRanked = true,
                            TournamentId = tournaments[2].Id,
                            MatchFormat = MatchFormat.Singles,
                            WinningSide = WinningSide.None,
                            CreatedDate = DateTime.UtcNow
                        };
                        round2Matches.Add(match);
                    }

                    context.Matches.AddRange(round2Matches);
                    await context.SaveChangesAsync();

                    // Tạo TournamentMatch cho Round 2 và link NextMatchId
                    for (int i = 0; i < round2Matches.Count; i++)
                    {
                        tournamentMatches.Add(new TournamentMatch
                        {
                            TournamentId = tournaments[2].Id,
                            MatchId = round2Matches[i].Id,
                            Round = 2,
                            BracketGroup = "WinnerBracket"
                        });
                        // Link round 1 matches to round 2
                        tournamentMatches[i * 2].NextMatchId = round2Matches[i].Id;
                        tournamentMatches[i * 2 + 1].NextMatchId = round2Matches[i].Id;
                    }

                    // Round 3 (Semi-finals) - 2 trận
                    var round3Matches = new List<Match>();
                    for (int i = 0; i < 2; i++)
                    {
                        var match = new Match
                        {
                            Date = (tournaments[2].StartDate ?? DateTime.UtcNow).AddDays(2),
                            IsRanked = true,
                            TournamentId = tournaments[2].Id,
                            MatchFormat = MatchFormat.Singles,
                            WinningSide = WinningSide.None,
                            CreatedDate = DateTime.UtcNow
                        };
                        round3Matches.Add(match);
                    }

                    context.Matches.AddRange(round3Matches);
                    await context.SaveChangesAsync();

                    // TournamentMatch cho Round 3
                    int r2StartIdx = matchesInRound1;
                    for (int i = 0; i < round3Matches.Count; i++)
                    {
                        tournamentMatches.Add(new TournamentMatch
                        {
                            TournamentId = tournaments[2].Id,
                            MatchId = round3Matches[i].Id,
                            Round = 3,
                            BracketGroup = "WinnerBracket"
                        });
                        tournamentMatches[r2StartIdx + i * 2].NextMatchId = round3Matches[i].Id;
                        tournamentMatches[r2StartIdx + i * 2 + 1].NextMatchId = round3Matches[i].Id;
                    }

                    // Round 4 (Final) - 1 trận
                    var finalMatch = new Match
                    {
                        Date = (tournaments[2].StartDate ?? DateTime.UtcNow).AddDays(3),
                        IsRanked = true,
                        TournamentId = tournaments[2].Id,
                        MatchFormat = MatchFormat.Singles,
                        WinningSide = WinningSide.None,
                        CreatedDate = DateTime.UtcNow
                    };
                    context.Matches.Add(finalMatch);
                    await context.SaveChangesAsync();

                    int r3StartIdx = matchesInRound1 + matchesInRound1 / 2;
                    tournamentMatches.Add(new TournamentMatch
                    {
                        TournamentId = tournaments[2].Id,
                        MatchId = finalMatch.Id,
                        Round = 4,
                        BracketGroup = "WinnerBracket"
                    });
                    tournamentMatches[r3StartIdx].NextMatchId = finalMatch.Id;
                    tournamentMatches[r3StartIdx + 1].NextMatchId = finalMatch.Id;

                    context.TournamentMatches.AddRange(tournamentMatches);
                    await context.SaveChangesAsync();
                }
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
                    Content = "CLB xin chúc mừng:\n\n🥇 Vị trí 1: Lê Minh Khôi - 1400 ELO\n🥈 Vị trí 2: Dương Trọng Nghĩa - 1380 ELO\n🥉 Vị trí 3: Tô Quang Huy - 1370 ELO\n\nChúc mừng các bạn! Hẹn gặp lại trong giải tháng 1!",
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
        var depositCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Nạp tiền ví")
                            ?? await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Nạp tiền");
        var bookingCategory = await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Phí đặt sân")
                            ?? await context.TransactionCategories.FirstOrDefaultAsync(c => c.Name == "Phí sân");
        
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
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

        // Seed Sample Tournament
        if (!await context.Tournaments.AnyAsync())
        {
            var creator = await context.Members.FirstAsync();
            var tournament = new Tournament
            {
                Title = "Giải Giao Hữu Tháng 1",
                Type = TournamentType.MiniGame,
                GameMode = GameMode.TeamBattle,
                Status = TournamentStatus.Open,
                Config_TargetWins = 5,
                EntryFee = 50000,
                PrizePool = 0,
                CreatedBy = creator.Id,
                StartDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow
            };
            
            context.Tournaments.Add(tournament);
            await context.SaveChangesAsync();
        }

        // Seed Initial Club Fund
        if (!await context.Transactions.AnyAsync())
        {
            var admin = await context.Members.FirstAsync();
            var category = await context.TransactionCategories.FirstAsync(c => c.Type == TransactionType.Income);
            
            var initialFund = new Transaction
            {
                Date = DateTime.UtcNow,
                Amount = 5000000,
                Description = "Quỹ khởi tạo CLB",
                CategoryId = category.Id,
                CreatedBy = admin.Id,
                CreatedDate = DateTime.UtcNow
            };
            
            context.Transactions.Add(initialFund);
            await context.SaveChangesAsync();
        }
    }
}
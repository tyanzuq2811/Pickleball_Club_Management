using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCM.Domain.Entities;
using PCM.Domain.Enums;

namespace PCM.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Tạo database nếu chưa có
        await context.Database.MigrateAsync();

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

        // Seed Sample Members
        if (!await context.Members.AnyAsync(m => m.Email != adminEmail))
        {
            var sampleMembers = new List<(string Name, string Email, double Rank)>
            {
                ("Nguyễn Văn A", "nguyenvana@pcm.com", 1300),
                ("Trần Thị B", "tranthib@pcm.com", 1250),
                ("Lê Văn C", "levanc@pcm.com", 1400),
                ("Phạm Thị D", "phamthid@pcm.com", 1350),
                ("Hoàng Văn E", "hoangvane@pcm.com", 1200),
                ("Vũ Thị F", "vuthif@pcm.com", 1280)
            };

            foreach (var (name, email, rank) in sampleMembers)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                
                await userManager.CreateAsync(user, "Member@123");
                await userManager.AddToRoleAsync(user, "Member");

                var member = new Member
                {
                    UserId = user.Id,
                    FullName = name,
                    Email = email,
                    PhoneNumber = $"09{new Random().Next(10000000, 99999999)}",
                    DateOfBirth = DateTime.UtcNow.AddYears(-new Random().Next(20, 40)),
                    JoinDate = DateTime.UtcNow.AddMonths(-new Random().Next(1, 12)),
                    RankELO = rank,
                    WalletBalance = 500000,
                    IsActive = true
                };
                
                context.Members.Add(member);
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
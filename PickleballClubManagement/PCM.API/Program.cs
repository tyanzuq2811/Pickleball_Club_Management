using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PCM.Application.Interfaces;
using PCM.Application.Services;
using PCM.Domain.Interfaces;
using PCM.Infrastructure.Data;
using PCM.Infrastructure.Repositories;
using PCM.Infrastructure.Services;
using PCM.API.Middleware;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    try
    {
        var configuration = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
        var options = ConfigurationOptions.Parse(configuration);
        options.AbortOnConnectFail = false; // Không crash nếu Redis chưa chạy
        options.ConnectTimeout = 5000;
        return ConnectionMultiplexer.Connect(options);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Could not connect to Redis: {ex.Message}");
        Console.WriteLine("Application will continue without Redis caching.");
        // Return a dummy multiplexer để app không crash
        return ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false,connectTimeout=1");
    }
});

// 4. Hangfire
builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

// 5. JWT Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
    
    // Config cho SignalR auth qua query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// 6. Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRedisService, RedisService>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ICourtService, CourtService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<INotificationService, PCM.API.Services.NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<BackgroundJobService>();

// 7. AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 8. SignalR
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PCM API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        builder => builder
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure Pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();

// Map SignalR Hubs
app.MapHub<PCM.API.Hubs.ScoreboardHub>("/hubs/scoreboard");

// Debug: In giá trị HangfireConnection
Console.WriteLine("HangfireConnection: " + builder.Configuration.GetConnectionString("HangfireConnection"));

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.Initialize(services);
}

// Configure Hangfire Jobs
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

// Auto-cancel expired pending bookings every 2 minutes
recurringJobManager.AddOrUpdate<BackgroundJobService>(
    "cancel-expired-bookings",
    x => x.CancelExpiredPendingBookings(),
    "*/2 * * * *");

// Send booking reminders every hour
recurringJobManager.AddOrUpdate<BackgroundJobService>(
    "send-booking-reminders",
    x => x.SendUpcomingBookingReminders(),
    Cron.Hourly);

// Clean old activity logs daily at 2 AM
recurringJobManager.AddOrUpdate<BackgroundJobService>(
    "clean-old-logs",
    x => x.CleanOldActivityLogs(),
    Cron.Daily(2));

// Update tournament standings daily at 1 AM
recurringJobManager.AddOrUpdate<BackgroundJobService>(
    "update-tournament-standings",
    x => x.UpdateTournamentStandings(),
    Cron.Daily(1));

app.Run();
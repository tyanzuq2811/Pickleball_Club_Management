using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Matches;
using PCM.Application.DTOs.Tournaments;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Enums;
using PCM.Domain.Interfaces;
using StackExchange.Redis;

namespace PCM.Application.Services;

public class TournamentService : ITournamentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisService _redisService;

    public TournamentService(IUnitOfWork unitOfWork, IRedisService redisService)
    {
        _unitOfWork = unitOfWork;
        _redisService = redisService;
    }

    public async Task<ApiResponse<TournamentDto>> GetByIdAsync(int id)
    {
        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
        if (tournament == null) return ApiResponse<TournamentDto>.ErrorResponse("Tournament not found");
        var dto = new TournamentDto
        {
            Id = tournament.Id,
            Title = tournament.Title,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            Status = tournament.Status,
            Type = tournament.Type,
            GameMode = tournament.GameMode,
            ConfigTargetWins = tournament.Config_TargetWins,
            CurrentScoreTeamA = tournament.CurrentScore_TeamA,
            CurrentScoreTeamB = tournament.CurrentScore_TeamB,
            EntryFee = tournament.EntryFee,
            PrizePool = tournament.PrizePool,
            CreatedBy = tournament.CreatedBy,
            CreatedDate = tournament.CreatedDate
        };
        return ApiResponse<TournamentDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<PagedResult<TournamentDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var list = await _unitOfWork.Tournaments.GetAllAsync();
        var total = list.Count();
        var items = list.OrderByDescending(x => x.StartDate).Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(t => new TournamentDto
            {
                Id = t.Id,
                Title = t.Title,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status,
                Type = t.Type,
                GameMode = t.GameMode,
                ConfigTargetWins = t.Config_TargetWins,
                CurrentScoreTeamA = t.CurrentScore_TeamA,
                CurrentScoreTeamB = t.CurrentScore_TeamB,
                EntryFee = t.EntryFee,
                PrizePool = t.PrizePool,
                CreatedBy = t.CreatedBy,
                CreatedDate = t.CreatedDate
            }).ToList();

        var result = new PagedResult<TournamentDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        return ApiResponse<PagedResult<TournamentDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<TournamentDto>> CreateAsync(TournamentCreateDto dto, int createdBy)
    {
        var tournament = new Tournament
        {
            Title = dto.Title,
            StartDate = dto.StartDate,
            Status = TournamentStatus.Open,
            Type = dto.Type,
            GameMode = dto.GameMode,
            Config_TargetWins = dto.ConfigTargetWins,
            EntryFee = dto.EntryFee,
            PrizePool = dto.PrizePool,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };
        await _unitOfWork.Tournaments.AddAsync(tournament);
        await _unitOfWork.SaveChangesAsync();
        var res = new TournamentDto
        {
            Id = tournament.Id,
            Title = tournament.Title,
            StartDate = tournament.StartDate,
            Status = tournament.Status,
            Type = tournament.Type,
            GameMode = tournament.GameMode,
            ConfigTargetWins = tournament.Config_TargetWins,
            EntryFee = tournament.EntryFee,
            PrizePool = tournament.PrizePool,
            CreatedBy = tournament.CreatedBy,
            CreatedDate = tournament.CreatedDate
        };
        return ApiResponse<TournamentDto>.SuccessResponse(res, "Tournament created");
    }

    public async Task<ApiResponse<TournamentDto>> UpdateAsync(int id, TournamentUpdateDto dto)
    {
        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(id);
        if (tournament == null) return ApiResponse<TournamentDto>.ErrorResponse("Tournament not found");

        if (dto.Title != null) tournament.Title = dto.Title;
        if (dto.Status.HasValue) tournament.Status = dto.Status.Value;
        if (dto.StartDate.HasValue) tournament.StartDate = dto.StartDate;
        if (dto.EndDate.HasValue) tournament.EndDate = dto.EndDate;

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<ApiResponse<bool>> JoinTournamentAsync(int tournamentId, int memberId)
    {
        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        if (tournament == null) return ApiResponse<bool>.ErrorResponse("Tournament not found");

        var exists = await _unitOfWork.Participants.AnyAsync(p => p.TournamentId == tournamentId && p.MemberId == memberId);
        if (exists) return ApiResponse<bool>.ErrorResponse("Already joined");

        var participant = new Participant
        {
            TournamentId = tournamentId,
            MemberId = memberId,
            Status = ParticipantStatus.Confirmed,
            JoinedDate = DateTime.UtcNow,
            EntryFeeAmount = tournament.EntryFee
        };

        await _unitOfWork.Participants.AddAsync(participant);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Joined successfully");
    }

    public async Task<ApiResponse<bool>> AutoDivideTeamsAsync(int tournamentId)
    {
        // Logic giả lập: Chia đều A và B
        var participants = await _unitOfWork.Participants.FindAsync(p => p.TournamentId == tournamentId);
        int count = 0;
        foreach (var p in participants)
        {
            p.Team = (count % 2 == 0) ? TeamSide.TeamA : TeamSide.TeamB;
            count++;
        }
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Teams divided");
    }

    public async Task<ApiResponse<bool>> UpdateScoreAsync(int tournamentId, int teamAScore, int teamBScore)
    {
        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        if (tournament == null) return ApiResponse<bool>.ErrorResponse("Tournament not found");

        tournament.CurrentScore_TeamA = teamAScore;
        tournament.CurrentScore_TeamB = teamBScore;
        
        // Check win condition for TeamBattle
        if (tournament.GameMode == GameMode.TeamBattle && tournament.Config_TargetWins.HasValue)
        {
            if (teamAScore >= tournament.Config_TargetWins || teamBScore >= tournament.Config_TargetWins)
            {
                tournament.Status = TournamentStatus.Finished;
                
                // Prize Distribution for TeamBattle
                if (tournament.PrizePool > 0)
                {
                    var winningTeam = teamAScore > teamBScore ? TeamSide.TeamA : TeamSide.TeamB;
                    var winners = await _unitOfWork.Participants.FindAsync(p => p.TournamentId == tournamentId && p.Team == winningTeam && p.Status == ParticipantStatus.Confirmed);
                    
                    if (winners.Any())
                    {
                        var prizePerPerson = tournament.PrizePool / winners.Count();
                        foreach (var winner in winners)
                        {
                            var member = await _unitOfWork.Members.GetByIdAsync(winner.MemberId);
                            if (member != null)
                            {
                                member.WalletBalance += prizePerPerson;
                                _unitOfWork.Members.Update(member);
                                
                                await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                                {
                                    MemberId = member.Id,
                                    Amount = prizePerPerson,
                                    CategoryId = 4, // Prize
                                    Type = WalletTransactionType.ReceivePrize,
                                    Date = DateTime.UtcNow,
                                    Description = $"Prize for tournament {tournament.Title}",
                                    Status = TransactionStatus.Success,
                                    ReferenceId = tournament.Id.ToString()
                                });
                            }
                        }
                        
                        // Club Expense Transaction
                        await _unitOfWork.Transactions.AddAsync(new Transaction
                        {
                            Date = DateTime.UtcNow,
                            Amount = -tournament.PrizePool,
                            Description = $"Prize distribution for {tournament.Title}",
                            CategoryId = 4, // Prize
                            CreatedBy = null
                        });
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Score updated");
    }

    public async Task<ApiResponse<bool>> UpdateMatchResultAsync(int matchId, int team1Score, int team2Score, int setNumber, bool isFinal)
    {
        var match = await _unitOfWork.Matches.GetByIdAsync(matchId);
        if (match == null) return ApiResponse<bool>.ErrorResponse("Match not found");
        
        // Nếu trận đấu đã có kết quả thắng thua rồi thì không cho update nữa để tránh spam ELO
        if (match.WinningSide != WinningSide.None)
            return ApiResponse<bool>.ErrorResponse("Match already finished");

        var score = new MatchScore
        {
            MatchId = matchId,
            SetNumber = setNumber,
            Team1Score = team1Score,
            Team2Score = team2Score,
            IsFinalSet = isFinal
        };

        await _unitOfWork.MatchScores.AddAsync(score);
        
        // Nếu là set cuối (trận đấu kết thúc) -> Tính ELO
        if (isFinal)
        {
            // Xác định người thắng dựa trên tổng set hoặc logic đơn giản là người thắng set cuối (tùy luật, ở đây giả sử set cuối quyết định hoặc logic FE gửi lên)
            // Để đơn giản cho bài toán: So sánh tổng điểm các set hoặc nhận định từ FE. 
            // Ở đây ta tạm quy định: Nếu team1Score > team2Score ở set cuối và là final -> Team 1 thắng.
            
            match.WinningSide = team1Score > team2Score ? WinningSide.Team1 : WinningSide.Team2;
            
            if (match.IsRanked)
            {
                // Lấy thông tin người chơi (Check null vì ID giờ là nullable)
                var p1 = match.Team1_Player1Id.HasValue ? await _unitOfWork.Members.GetByIdAsync(match.Team1_Player1Id.Value) : null;
                var p2 = match.Team2_Player1Id.HasValue ? await _unitOfWork.Members.GetByIdAsync(match.Team2_Player1Id.Value) : null;

                if (p1 != null && p2 != null)
                {
                    // Tính ELO (K-factor = 32)
                    double kFactor = 32;
                    double expectedP1 = 1 / (1 + Math.Pow(10, (p2.RankELO - p1.RankELO) / 400));
                    double expectedP2 = 1 / (1 + Math.Pow(10, (p1.RankELO - p2.RankELO) / 400));

                    double actualP1 = match.WinningSide == WinningSide.Team1 ? 1 : 0;
                    double actualP2 = match.WinningSide == WinningSide.Team2 ? 1 : 0;

                    double changeP1 = kFactor * (actualP1 - expectedP1);
                    double changeP2 = kFactor * (actualP2 - expectedP2);

                    // Cập nhật DB
                    p1.RankELO += changeP1;
                    p2.RankELO += changeP2;
                    
                    p1.TotalMatches++;
                    p2.TotalMatches++;
                    
                    if (match.WinningSide == WinningSide.Team1) p1.WinMatches++;
                    else p2.WinMatches++;

                    match.EloChange = Math.Abs(changeP1);

                    _unitOfWork.Members.Update(p1);
                    _unitOfWork.Members.Update(p2);

                    // Cập nhật Redis Leaderboard
                    try
                    {
                        await _redisService.SortedSetAddAsync("leaderboard:elo", p1.Id.ToString(), p1.RankELO);
                        await _redisService.SortedSetAddAsync("leaderboard:elo", p2.Id.ToString(), p2.RankELO);
                        await _redisService.DeleteAsync("leaderboard:top:10"); // Xóa cache top ranking cũ
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Redis update failed: {ex.Message}");
                    }
                }
            }

            // Bracket Advancement: Đẩy người thắng vào vòng sau
            var tournamentMatch = await _unitOfWork.TournamentMatches.FirstOrDefaultAsync(tm => tm.MatchId == matchId);
            if (tournamentMatch != null && tournamentMatch.NextMatchId.HasValue)
            {
                var nextMatch = await _unitOfWork.Matches.GetByIdAsync(tournamentMatch.NextMatchId.Value);
                if (nextMatch != null)
                {
                    var winnerId = match.WinningSide == WinningSide.Team1 ? match.Team1_Player1Id : match.Team2_Player1Id;
                    if (nextMatch.Team1_Player1Id == null)
                    {
                        nextMatch.Team1_Player1Id = winnerId;
                    }
                    else if (nextMatch.Team2_Player1Id == null)
                    {
                        nextMatch.Team2_Player1Id = winnerId;
                    }
                    _unitOfWork.Matches.Update(nextMatch);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, "Match score updated");
    }

    public async Task<ApiResponse<List<TournamentDto>>> GetOpenTournamentsAsync()
    {
        var list = await _unitOfWork.Tournaments.FindAsync(t => t.Status == TournamentStatus.Open);
        var dtos = list.OrderBy(t => t.StartDate)
            .Select(t => new TournamentDto
            {
                Id = t.Id,
                Title = t.Title,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status,
                Type = t.Type,
                GameMode = t.GameMode,
                ConfigTargetWins = t.Config_TargetWins,
                EntryFee = t.EntryFee,
                PrizePool = t.PrizePool,
                CreatedBy = t.CreatedBy,
                CreatedDate = t.CreatedDate
            }).ToList();
        return ApiResponse<List<TournamentDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<bool>> GenerateBracketAsync(int tournamentId)
    {
        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        if (tournament == null) return ApiResponse<bool>.ErrorResponse("Tournament not found");

        // Check if bracket already exists
        var existingMatches = await _unitOfWork.TournamentMatches.AnyAsync(tm => tm.TournamentId == tournamentId);
        if (existingMatches) return ApiResponse<bool>.ErrorResponse("Bracket already generated");

        var participants = (await _unitOfWork.Participants.FindAsync(p => p.TournamentId == tournamentId && p.Status == ParticipantStatus.Confirmed)).ToList();
        
        if (participants.Count < 2) return ApiResponse<bool>.ErrorResponse("Not enough participants");

        // Simple Single Elimination - Round 1
        // Shuffle participants
        var rng = new Random();
        var shuffled = participants.OrderBy(a => rng.Next()).ToList();

        // Calculate rounds
        // Example: 8 participants -> 4 matches (R1) -> 2 matches (R2) -> 1 match (R3)
        var rounds = new List<List<TournamentMatch>>();
        var currentParticipants = shuffled.Select(p => (int?)p.MemberId).ToList();
        int roundNumber = 1;

        // Round 1
        while (currentParticipants.Count > 1)
        {
            var roundMatches = new List<TournamentMatch>();
            var nextRoundParticipants = new List<int?>(); // Just placeholders count

            for (int i = 0; i < currentParticipants.Count; i += 2)
            {
                var p1 = currentParticipants[i];
                var p2 = (i + 1 < currentParticipants.Count) ? currentParticipants[i + 1] : null;

                var match = new Match
                {
                    Date = tournament.StartDate?.AddDays(roundNumber - 1) ?? DateTime.UtcNow.AddDays(roundNumber - 1),
                    IsRanked = true,
                    TournamentId = tournamentId,
                    MatchFormat = MatchFormat.Singles,
                    Team1_Player1Id = p1,
                    Team2_Player1Id = p2,
                    WinningSide = WinningSide.None,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Matches.AddAsync(match);
                await _unitOfWork.SaveChangesAsync();

                var tm = new TournamentMatch
                {
                    TournamentId = tournamentId,
                    Round = roundNumber,
                    MatchId = match.Id,
                    BracketGroup = "WinnerBracket"
                };
                await _unitOfWork.TournamentMatches.AddAsync(tm);
                roundMatches.Add(tm);
                
                nextRoundParticipants.Add(null); // Placeholder for winner
            }

            rounds.Add(roundMatches);
            currentParticipants = nextRoundParticipants;
            roundNumber++;
        }

        // Link Matches (NextMatchId)
        // Loop through rounds except the last one
        for (int r = 0; r < rounds.Count - 1; r++)
        {
            var currentRound = rounds[r];
            var nextRound = rounds[r + 1];

            for (int i = 0; i < currentRound.Count; i++)
            {
                var currentMatch = currentRound[i];
                var parentIndex = i / 2; // 2 matches feed into 1 match in next round
                
                if (parentIndex < nextRound.Count)
                {
                    currentMatch.NextMatchId = nextRound[parentIndex].MatchId;
                    // Update DB
                    // Note: EF Core tracks entities, so modifying property is enough if we call SaveChanges at end
                }
            }
        }

        tournament.Status = TournamentStatus.Ongoing;
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Bracket generated");
    }

    public async Task<ApiResponse<TournamentBracketDto>> GetBracketAsync(int tournamentId)
    {
        var tournament = await _unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        if (tournament == null) return ApiResponse<TournamentBracketDto>.ErrorResponse("Tournament not found");

        var tournamentMatches = await _unitOfWork.TournamentMatches.FindAsync(tm => tm.TournamentId == tournamentId);
        if (!tournamentMatches.Any()) return ApiResponse<TournamentBracketDto>.ErrorResponse("Bracket not generated yet");

        var matchIds = tournamentMatches.Select(tm => tm.MatchId).ToList();
        var matches = await _unitOfWork.Matches.FindAsync(m => matchIds.Contains(m.Id));
        
        // Load members info
        var memberIds = matches.SelectMany(m => new[] { m.Team1_Player1Id, m.Team2_Player1Id })
                               .Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        var members = await _unitOfWork.Members.FindAsync(m => memberIds.Contains(m.Id));
        var memberDict = members.ToDictionary(m => m.Id, m => m.FullName);

        var rounds = tournamentMatches.GroupBy(tm => tm.Round)
            .OrderBy(g => g.Key)
            .Select(g => new BracketRoundDto
            {
                RoundNumber = g.Key,
                Matches = g.Select(tm => {
                    var m = matches.FirstOrDefault(x => x.Id == tm.MatchId);
                    return new BracketMatchDto 
                    {
                        MatchId = m!.Id,
                        Team1Player1 = m.Team1_Player1Id.HasValue ? memberDict.GetValueOrDefault(m.Team1_Player1Id.Value) : "TBD",
                        Team2Player1 = m.Team2_Player1Id.HasValue ? memberDict.GetValueOrDefault(m.Team2_Player1Id.Value) : "TBD",
                        WinningSide = m.WinningSide,
                        NextMatchId = tm.NextMatchId
                    };
                }).ToList()
            }).ToList();

        return ApiResponse<TournamentBracketDto>.SuccessResponse(new TournamentBracketDto 
        { 
            TournamentId = tournamentId, 
            Rounds = rounds 
        });
    }

    public async Task<ApiResponse<List<MatchDto>>> GetAllMatchesAsync()
    {
        var matches = await _unitOfWork.Matches.GetAllAsync();
        
        // Load members info for names
        var memberIds = matches.SelectMany(m => new[] { m.Team1_Player1Id, m.Team2_Player1Id, m.Team1_Player2Id, m.Team2_Player2Id })
                               .Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        var members = await _unitOfWork.Members.FindAsync(m => memberIds.Contains(m.Id));
        var memberDict = members.ToDictionary(m => m.Id, m => m.FullName);

        var dtos = new List<MatchDto>();
        foreach (var m in matches.OrderByDescending(x => x.Date))
        {
            // Tính tổng điểm hiện tại từ MatchScores
            var scores = await _unitOfWork.MatchScores.FindAsync(s => s.MatchId == m.Id);
            var t1Score = scores.Sum(s => s.Team1Score); // Hoặc logic tính set thắng
            var t2Score = scores.Sum(s => s.Team2Score);

            string t1Name = m.Team1_Player1Id.HasValue ? memberDict.GetValueOrDefault(m.Team1_Player1Id.Value, "TBD") : "TBD";
            string t2Name = m.Team2_Player1Id.HasValue ? memberDict.GetValueOrDefault(m.Team2_Player1Id.Value, "TBD") : "TBD";

            dtos.Add(new MatchDto
            {
                Id = m.Id,
                Team1Name = t1Name,
                Team2Name = t2Name,
                Team1Score = t1Score,
                Team2Score = t2Score,
                Status = m.WinningSide != WinningSide.None ? "Finished" : "Ongoing",
                Date = m.Date
            });
        }

        return ApiResponse<List<MatchDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<List<object>>> GetLiveMatchesAsync()
    {
        // Lấy các giải đấu đang diễn ra
        var tournaments = await _unitOfWork.Tournaments.FindAsync(t => t.Status == TournamentStatus.Ongoing);
        var result = new List<object>();

        foreach (var tournament in tournaments)
        {
            result.Add(new
            {
                tournamentId = tournament.Id,
                tournamentTitle = tournament.Title,
                type = tournament.Type.ToString(),
                gameMode = tournament.GameMode.ToString(),
                teamAScore = tournament.CurrentScore_TeamA,
                teamBScore = tournament.CurrentScore_TeamB,
                targetWins = tournament.Config_TargetWins
            });
        }

        return ApiResponse<List<object>>.SuccessResponse(result);
    }
}
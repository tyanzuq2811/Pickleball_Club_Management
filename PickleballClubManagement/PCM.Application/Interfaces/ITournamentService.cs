using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Matches;
using PCM.Application.DTOs.Tournaments;

namespace PCM.Application.Interfaces;

public interface ITournamentService
{
    Task<ApiResponse<TournamentDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResult<TournamentDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse<TournamentDto>> CreateAsync(TournamentCreateDto dto, int createdBy);
    Task<ApiResponse<TournamentDto>> UpdateAsync(int id, TournamentUpdateDto dto);
    Task<ApiResponse<bool>> JoinTournamentAsync(int tournamentId, int memberId);
    Task<ApiResponse<bool>> AutoDivideTeamsAsync(int tournamentId);
    Task<ApiResponse<bool>> UpdateScoreAsync(int tournamentId, int teamAScore, int teamBScore);
    Task<ApiResponse<bool>> UpdateMatchResultAsync(int matchId, int team1Score, int team2Score, int setNumber, bool isFinal);
    Task<ApiResponse<List<TournamentDto>>> GetOpenTournamentsAsync();
    Task<ApiResponse<bool>> GenerateBracketAsync(int tournamentId);
    Task<ApiResponse<TournamentBracketDto>> GetBracketAsync(int tournamentId);
    Task<ApiResponse<List<MatchDto>>> GetAllMatchesAsync();
    Task<ApiResponse<List<object>>> GetLiveMatchesAsync();
    Task<ApiResponse<List<ParticipantDto>>> GetParticipantsAsync(int tournamentId);
}
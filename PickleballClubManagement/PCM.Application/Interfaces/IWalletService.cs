using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Wallet;

namespace PCM.Application.Interfaces;

public interface IWalletService
{
    Task<ApiResponse<bool>> PayBookingAsync(int memberId, decimal amount, int bookingId);
    Task<ApiResponse<decimal>> GetBalanceAsync(int memberId);
    Task<ApiResponse<PagedResult<WalletTransactionDto>>> GetTransactionsAsync(int memberId, int pageNumber, int pageSize);
    Task<ApiResponse<WalletTransactionDto>> CreateDepositRequestAsync(int memberId, DepositRequestDto dto);
    Task<ApiResponse<bool>> ApproveDepositAsync(ApproveDepositDto dto);
}
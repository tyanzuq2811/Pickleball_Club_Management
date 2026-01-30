using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Wallet;

namespace PCM.Application.Interfaces;

public interface IWalletService
{
    Task<ApiResponse<List<WalletTransactionDto>>> GetMemberTransactionsAsync(string userId);
    Task<ApiResponse<bool>> RequestDepositAsync(string userId, decimal amount);
    Task<ApiResponse<List<WalletTransactionDto>>> GetPendingDepositsAsync();
    Task<ApiResponse<bool>> ApproveDepositAsync(int id);
    Task<ApiResponse<bool>> RejectDepositAsync(int id);
    Task<ApiResponse<bool>> PayBookingAsync(int memberId, decimal amount, string bookingReference);
}
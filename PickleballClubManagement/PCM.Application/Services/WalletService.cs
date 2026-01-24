using PCM.Application.DTOs.Common;
using PCM.Application.DTOs.Wallet;
using PCM.Application.Interfaces;
using PCM.Domain.Entities;
using PCM.Domain.Enums;
using PCM.Domain.Interfaces;

namespace PCM.Application.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;

    public WalletService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<WalletTransactionDto>>> GetMemberTransactionsAsync(string userId)
    {
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null) return ApiResponse<List<WalletTransactionDto>>.ErrorResponse("Member not found");

        var transactions = await _unitOfWork.WalletTransactions.FindAsync(t => t.MemberId == member.Id);
        
        var dtos = transactions.OrderByDescending(t => t.Date).Select(t => new WalletTransactionDto
        {
            Id = t.Id,
            Date = t.Date,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description ?? "",
            Status = t.Status,
            MemberName = member.FullName,
            CategoryName = "Ví cá nhân" // Tạm thời
        }).ToList();

        return ApiResponse<List<WalletTransactionDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<bool>> RequestDepositAsync(string userId, decimal amount)
    {
        var member = await _unitOfWork.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null) return ApiResponse<bool>.ErrorResponse("Member not found");

        if (amount <= 0) return ApiResponse<bool>.ErrorResponse("Amount must be greater than 0");

        var transaction = new WalletTransaction
        {
            MemberId = member.Id,
            Amount = amount,
            Date = DateTime.UtcNow,
            Type = WalletTransactionType.Deposit,
            Status = TransactionStatus.Pending,
            Description = "Nạp tiền vào ví",
            CategoryId = 1, // Giả sử ID 1 là Nạp tiền
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.WalletTransactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Deposit request created");
    }

    public async Task<ApiResponse<List<WalletTransactionDto>>> GetPendingDepositsAsync()
    {
        var transactions = await _unitOfWork.WalletTransactions.FindAsync(t => t.Status == TransactionStatus.Pending && t.Type == WalletTransactionType.Deposit);
        
        // Cần load thông tin Member để hiển thị tên
        var memberIds = transactions.Select(t => t.MemberId).Distinct().ToList();
        var members = await _unitOfWork.Members.FindAsync(m => memberIds.Contains(m.Id));
        var memberDict = members.ToDictionary(m => m.Id, m => m.FullName);

        var dtos = transactions.OrderBy(t => t.Date).Select(t => new WalletTransactionDto
        {
            Id = t.Id,
            Date = t.Date,
            Amount = t.Amount,
            Type = t.Type,
            Description = t.Description ?? "",
            Status = t.Status,
            MemberName = memberDict.GetValueOrDefault(t.MemberId, "Unknown"),
            CategoryName = "Nạp tiền"
        }).ToList();

        return ApiResponse<List<WalletTransactionDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<bool>> ApproveDepositAsync(int id)
    {
        var transaction = await _unitOfWork.WalletTransactions.GetByIdAsync(id);
        if (transaction == null) return ApiResponse<bool>.ErrorResponse("Transaction not found");
        if (transaction.Status != TransactionStatus.Pending) return ApiResponse<bool>.ErrorResponse("Transaction is not pending");

        transaction.Status = TransactionStatus.Success;
        var member = await _unitOfWork.Members.GetByIdAsync(transaction.MemberId);
        if (member != null)
        {
            member.WalletBalance += transaction.Amount;
            _unitOfWork.Members.Update(member);
        }

        _unitOfWork.WalletTransactions.Update(transaction);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Deposit approved");
    }

    public async Task<ApiResponse<bool>> PayBookingAsync(int memberId, decimal amount, string bookingReference)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(memberId);
        if (member == null) return ApiResponse<bool>.ErrorResponse("Member not found");

        if (member.WalletBalance < amount)
            return ApiResponse<bool>.ErrorResponse("Số dư không đủ để thanh toán");

        member.WalletBalance -= amount;
        _unitOfWork.Members.Update(member);

        var transaction = new WalletTransaction
        {
            MemberId = member.Id,
            Amount = -amount,
            Date = DateTime.UtcNow,
            Type = WalletTransactionType.PayBooking,
            Status = TransactionStatus.Success,
            Description = $"Thanh toán đặt sân {bookingReference}",
            ReferenceId = bookingReference,
            CategoryId = 2, // Phí sân
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.WalletTransactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Thanh toán thành công");
    }
}
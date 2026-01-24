using System.Security.Cryptography;
using System.Text;
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
    private readonly IActivityLogService _activityLogService;

    public WalletService(IUnitOfWork unitOfWork, IActivityLogService activityLogService)
    {
        _unitOfWork = unitOfWork;
        _activityLogService = activityLogService;
    }

    public async Task<ApiResponse<bool>> PayBookingAsync(int memberId, decimal amount, int bookingId)
    {
        try
        {
            var member = await _unitOfWork.Members.GetByIdAsync(memberId);
            if (member == null) return ApiResponse<bool>.ErrorResponse("Member not found");

            if (member.WalletBalance < amount)
                return ApiResponse<bool>.ErrorResponse("Insufficient balance");

            // Deduct money
            member.WalletBalance -= amount;
            
            // Log transaction
            var transaction = new WalletTransaction
            {
                MemberId = memberId,
                Amount = -amount,
                CategoryId = 2, // Fee
                Type = WalletTransactionType.PayBooking,
                Date = DateTime.UtcNow,
                Description = $"Payment for booking #{bookingId}",
                Status = TransactionStatus.Success,
                ReferenceId = bookingId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            transaction.EncryptedSignature = ComputeSignature(transaction);
            await _unitOfWork.WalletTransactions.AddAsync(transaction);
            _unitOfWork.Members.Update(member);
            
            // SaveChanges commits both operations
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(ex.Message);
        }
    }

    public async Task<ApiResponse<decimal>> GetBalanceAsync(int memberId)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(memberId);
        if (member == null) return ApiResponse<decimal>.ErrorResponse("Member not found");
        return ApiResponse<decimal>.SuccessResponse(member.WalletBalance);
    }

    public async Task<ApiResponse<PagedResult<WalletTransactionDto>>> GetTransactionsAsync(int memberId, int pageNumber, int pageSize)
    {
        var transactions = await _unitOfWork.WalletTransactions.FindAsync(t => t.MemberId == memberId);
        var total = transactions.Count();
        var items = transactions.OrderByDescending(t => t.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                MemberId = t.MemberId,
                Type = t.Type,
                Status = t.Status,
                Description = t.Description
            }).ToList();

        return ApiResponse<PagedResult<WalletTransactionDto>>.SuccessResponse(new PagedResult<WalletTransactionDto>
        {
            Items = items,
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<WalletTransactionDto>> CreateDepositRequestAsync(int memberId, DepositRequestDto dto)
    {
        var transaction = new WalletTransaction
        {
            MemberId = memberId,
            Amount = dto.Amount,
            CategoryId = 1, // Nạp tiền
            Type = WalletTransactionType.Deposit,
            Date = DateTime.UtcNow,
            Description = dto.Description,
            Status = TransactionStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        transaction.EncryptedSignature = ComputeSignature(transaction);
        await _unitOfWork.WalletTransactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<WalletTransactionDto>.SuccessResponse(new WalletTransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Status = transaction.Status
        }, "Deposit request created");
    }

    public async Task<ApiResponse<bool>> ApproveDepositAsync(ApproveDepositDto dto)
    {
        var transaction = await _unitOfWork.WalletTransactions.GetByIdAsync(dto.TransactionId);
        if (transaction == null) return ApiResponse<bool>.ErrorResponse("Transaction not found");
        
        if (transaction.Status != TransactionStatus.Pending)
            return ApiResponse<bool>.ErrorResponse("Transaction is not pending");

        if (dto.Approve)
        {
            transaction.Status = TransactionStatus.Success;
            transaction.Description += $" | Approved: {dto.Reason}";
            
            var member = await _unitOfWork.Members.GetByIdAsync(transaction.MemberId);
            if (member != null)
            {
                member.WalletBalance += transaction.Amount;
                _unitOfWork.Members.Update(member);
            }
            
            await _activityLogService.LogAsync(member?.UserId ?? "System", "DEPOSIT_APPROVED", $"Deposit {transaction.Amount} approved", "WalletTransaction", transaction.Id);
        }
        else
        {
            transaction.Status = TransactionStatus.Failed;
            transaction.Description += $" | Rejected: {dto.Reason}";
        }

        // Update signature because status changed (optional depending on business rule, but good for integrity)
        // transaction.EncryptedSignature = ComputeSignature(transaction); 

        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true, dto.Approve ? "Deposit approved" : "Deposit rejected");
    }

    private string ComputeSignature(WalletTransaction transaction)
    {
        var rawData = $"{transaction.MemberId}-{transaction.Amount}-{transaction.Date:O}-{transaction.Type}-{transaction.ReferenceId}";
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(rawData);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
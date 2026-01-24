using PCM.Domain.Enums;

namespace PCM.Application.DTOs.Wallet;

public class WalletTransactionDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public WalletTransactionType Type { get; set; }
    public string? ReferenceId { get; set; }
    public string? Description { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class DepositRequestDto
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? ProofImageUrl { get; set; }
}

public class ApproveDepositDto
{
    public int TransactionId { get; set; }
    public bool Approve { get; set; }
    public string? Reason { get; set; }
}

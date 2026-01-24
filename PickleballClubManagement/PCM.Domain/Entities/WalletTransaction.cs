using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PCM.Domain.Enums;

namespace PCM.Domain.Entities;

[Table("189_WalletTransactions")]
public class WalletTransaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    public int MemberId { get; set; }

    public int CategoryId { get; set; }

    [Required]
    public WalletTransactionType Type { get; set; }

    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public TransactionStatus Status { get; set; }

    [MaxLength(500)]
    public string? EncryptedSignature { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("MemberId")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual TransactionCategory Category { get; set; } = null!;
}

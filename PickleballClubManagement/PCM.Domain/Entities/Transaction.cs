using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("189_Transactions")]
public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual TransactionCategory Category { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public virtual Member? Creator { get; set; }
}

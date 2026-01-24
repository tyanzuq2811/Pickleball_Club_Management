using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Domain.Entities;

[Table("189_RefreshTokens")]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string JwtId { get; set; } = string.Empty;

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime ExpiryDate { get; set; }
}

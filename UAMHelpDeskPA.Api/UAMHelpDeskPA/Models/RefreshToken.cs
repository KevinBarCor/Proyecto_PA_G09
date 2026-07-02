using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UamHelpDeskPA.Api.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAtUtc { get; set; }

        public bool IsRevoked { get; set; } = false;

        public DateTime? RevokedAtUtc { get; set; }

        [StringLength(200)]
        public string? RevokedReason { get; set; }

        [Required]
        public DateTime CreatedAtUtc { get; set; }
    }
}
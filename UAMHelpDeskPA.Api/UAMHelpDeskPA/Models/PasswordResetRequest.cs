using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UamHelpDeskPA.Api.Models
{
    public class PasswordResetRequest
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string SessionToken { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAtUtc { get; set; }

        public bool IsUsed { get; set; } = false;

        [Required]
        public DateTime CreatedAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }
    }
}
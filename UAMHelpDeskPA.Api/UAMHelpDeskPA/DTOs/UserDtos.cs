using System.ComponentModel.DataAnnotations;

namespace UamHelpDeskPA.Api.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateUserDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }
    }

    public record UserDto(
        int Id,
        int RoleId,
        string RoleName,
        string FirstName,
        string LastName,
        string Email,
        bool IsActive
    );
}

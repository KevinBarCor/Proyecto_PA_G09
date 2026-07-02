using System.ComponentModel.DataAnnotations;

namespace UamHelpDeskPA.Api.DTOs
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }
    }

    public class UpdateRoleDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }
    }

    public record RoleDto(
        int Id,
        string Name,
        string? Description,
        bool IsActive
    );
}
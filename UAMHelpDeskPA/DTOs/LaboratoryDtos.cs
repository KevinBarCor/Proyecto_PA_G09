using System.ComponentModel.DataAnnotations;

namespace UamHelpDeskPA.Api.DTOs
{
    public record LaboratoryDto(int id,string Name, string Building, int BuildingFloor, int Capacity, bool IsActive, IReadOnlyList<EquipmentDto>? Equipments);
    public class CreateLaboratoryDto
    {
        [Required,MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Building { get; set; } = string.Empty;

        [Required]
        public int BuildingFloor { get; set; }

        [Required]
        public int Capacity { get; set; }
    }
    public class UpdateLaboratoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Building { get; set; } = string.Empty;

        [Required]
        public int BuildingFloor { get; set; }

        [Required]
        public int Capacity { get; set; }
    }
}

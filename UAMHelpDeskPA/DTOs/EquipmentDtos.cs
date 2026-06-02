using System.ComponentModel.DataAnnotations;
using UamHelpDeskPA.Api.Models;
using UamHelpDeskPA.Models;

namespace UamHelpDeskPA.Api.DTOs
{
    public record EquipmentDto(int Id,int LaboratoryId,string LaboratoryName,string Code,string Brand,string Model,string SerialNumber,string Type,string Status,DateTime? PurchaseDate,bool IsActive);
    public class CreateEquipmentDto
    {
        [Required]
        public int LaboratoryId { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        public EquipmentTypes Type { get; set; }

        [Required]
        public EquipmentStatus Status { get; set; }

        public DateTime? PurchaseDate { get; set; }
    }
    public class UpdateEquipmentDto
    {
        [Required]
        public int LaboratoryId { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        public EquipmentTypes Type { get; set; }

        [Required]
        public EquipmentStatus Status { get; set; }

        public DateTime? PurchaseDate { get; set; }
    }
}
